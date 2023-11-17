

#region System Usings
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
#endregion

using Microsoft.IdentityModel.Tokens;
using log4net;

#region Bouncy Castle Usings
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;
#endregion

using Notary.Configuration;
using Notary.Interface.Service;
using Notary.Contract;

namespace Notary.Service
{
    public class EncryptionService : IEncryptionService
    {
        private NotaryConfiguration _config;
        private ILog _log;

        public EncryptionService(NotaryConfiguration config, ILog log)
        {
            _config = config;
            _log = log;
        }

        /// <summary>
        /// Add the Authority Key Identifier. According to http://www.alvestrand.no/objectid/2.5.29.35.html, this
        /// identifies the public key to be used to verify the signature on this certificate.
        /// In a certificate chain, this corresponds to the "Subject Key Identifier" on the *issuer* certificate.
        /// The Bouncy Castle documentation, at http://www.bouncycastle.org/wiki/display/JA1/X.509+Public+Key+Certificate+and+Certification+Request+Generation,
        /// shows how to create this from the issuing certificate. Since we're creating a self-signed certificate, we have to do this slightly differently.
        /// </summary>
        /// <param name="certificateGenerator">The object used to generate certificate</param>
        /// <param name="issuerDN">The issuer's distinguished name</param>
        /// <param name="issuerKeyPair">The issuer's key pair</param>
        /// <param name="issuerSerialNumber">The issuer's serial number</param>
        public void AddAuthorityKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                      X509Name issuerDN,
                                                      AsymmetricCipherKeyPair issuerKeyPair,
                                                      BigInteger issuerSerialNumber)
        {
            var authorityKeyIdentifierExtension =
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKeyPair.Public),
                    new GeneralNames(new GeneralName(issuerDN)),
                    issuerSerialNumber);
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifierExtension);
        }

        /// <summary>
        /// Add the "Extended Key Usage" extension, specifying (for example) "server authentication".
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="usages"></param>
        public void AddExtendedKeyUsage(X509V3CertificateGenerator certificateGenerator, KeyPurposeID[] usages)
        {
            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(usages));
        }

        /// <summary>
        /// Add the "Basic Constraints" extension.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="isCertificateAuthority"></param>
        public void AddBasicConstraints(X509V3CertificateGenerator certificateGenerator,
                                                bool isCertificateAuthority)
        {
            certificateGenerator.AddExtension(
                X509Extensions.BasicConstraints.Id, true, new BasicConstraints(isCertificateAuthority));
        }

        /// <summary>
        /// Add the Subject Key Identifier.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="subjectKeyPair"></param>
        public void AddSubjectKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                    AsymmetricCipherKeyPair subjectKeyPair)
        {
            var subjectKeyIdentifierExtension =
                new SubjectKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public));
            certificateGenerator.AddExtension(
                X509Extensions.SubjectKeyIdentifier.Id, false, subjectKeyIdentifierExtension);
        }

        /// <summary>
        /// Add the "Subject Alternative Names" extension. Note that you have to repeat
        /// the value from the "Subject Name" property.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="subjectAlternativeNames"></param>
        public void AddSubjectAlternativeNames(X509V3CertificateGenerator certificateGenerator,
                                                       IEnumerable<SubjectAlternativeName> subjectAlternativeNames)
        {
            var sanExtension = new DerSequence(subjectAlternativeNames.Select(n =>
            {
                int generalName = -1;
                switch (n.Kind)
                {
                    case SanKind.Dns:
                        generalName = GeneralName.DnsName;
                        break;
                    case SanKind.Email:
                        generalName = GeneralName.X400Address;
                        break;
                    case SanKind.IpAddress:
                        generalName = GeneralName.IPAddress;
                        break;
                    case SanKind.UserPrincipal:
                        generalName = GeneralName.DirectoryName;
                        break;
                    case SanKind.Uri:
                        generalName = GeneralName.UniformResourceIdentifier;
                        break;
                    default:
                        break;
                }

                return new GeneralName(generalName, n.Name);
            }).ToArray<Asn1Encodable>());

            certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, sanExtension);
        }

        public X509Certificate GenerateCertificate(List<SubjectAlternativeName> sanList, SecureRandom random, Algorithm alg,
            string subjectDn, AsymmetricCipherKeyPair subjectKeyPair, BigInteger subjectSn, string issuerDn, DateTime notAfter,
            AsymmetricCipherKeyPair issuerKeyPair, BigInteger issuerSn, bool isCA, KeyPurposeID[] usages)
        {
            var certGen = new X509V3CertificateGenerator();
            var subject = new X509Name(subjectDn);
            var issuer = new X509Name(issuerDn);
            var notBefore = DateTime.UtcNow;
            Asn1SignatureFactory signatureFactory = null;
            switch (alg)
            {
                case Algorithm.RSA:
                    signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerKeyPair.Private);
                    break;
                case Algorithm.EllipticCurve:
                    signatureFactory = new Asn1SignatureFactory("SHA256WithECDSA", issuerKeyPair.Private);
                    break;
            }

            certGen.SetPublicKey(subjectKeyPair.Public);
            certGen.SetSerialNumber(subjectSn);
            certGen.SetSubjectDN(subject);
            certGen.SetIssuerDN(issuer);
            certGen.SetNotBefore(notBefore);
            certGen.SetNotAfter(notAfter);

            AddAuthorityKeyIdentifier(certGen, issuer, issuerKeyPair, issuerSn);
            AddSubjectKeyIdentifier(certGen, subjectKeyPair);
            AddBasicConstraints(certGen, isCA);

            if (usages != null && usages.Any())
                AddExtendedKeyUsage(certGen, usages);

            if (sanList != null && sanList.Any())
                AddSubjectAlternativeNames(certGen, sanList);

            var bouncyCert = certGen.Generate(signatureFactory);

            return bouncyCert;
        }

        public string GenerateJwt(ClaimsIdentity claims, DateTime tokenExpiry)
        {
            byte[] key = LoadEncryptionKey();
            var ssk = new SymmetricSecurityKey(key);
            SigningCredentials signingCredentials = new SigningCredentials(ssk, SecurityAlgorithms.HmacSha256Signature);
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenDesc = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Subject = claims,
                NotBefore = DateTime.UtcNow,
                Issuer = _config.TokenSettings.Issuer,
                Audience = _config.TokenSettings.Audience,
                IssuedAt = DateTime.UtcNow
            };

            if (tokenExpiry != DateTime.MaxValue)
                tokenDesc.Expires = tokenExpiry;

            var t = jwtHandler.CreateToken(tokenDesc);
            string token = jwtHandler.WriteToken(t);

            return token;
        }

        public AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, Algorithm algorithm, EllipticCurve? curve, int? keyLength)
        {
            AsymmetricCipherKeyPair keyPair = null;

            if (algorithm == Algorithm.RSA)
            {
                var keyGenerationParameters = new KeyGenerationParameters(random, keyLength.Value);
                var keyPairGenerator = new RsaKeyPairGenerator();
                keyPairGenerator.Init(keyGenerationParameters);
                keyPair = keyPairGenerator.GenerateKeyPair();
            }
            else if (algorithm == Algorithm.EllipticCurve)
            {
                var strCurve = curve.Value.ToString().Insert(1, "-");
                var ecp = NistNamedCurves.GetByName(strCurve); //TODO: Refactor

                var fpCurve = (FpCurve)ecp.Curve;
                var domain = new ECDomainParameters(fpCurve, ecp.G, ecp.N, ecp.H);

                var keyGenerationParameters = new ECKeyGenerationParameters(domain, random);
                var ecGenerator = new ECKeyPairGenerator();
                ecGenerator.Init(keyGenerationParameters);

                keyPair = ecGenerator.GenerateKeyPair();
            }

            return keyPair;
        }

        /// <summary>
        /// Generate a cryptographically secure random number
        /// </summary>
        /// <returns>The cryptographically secure random number generated object</returns>
        public SecureRandom GetSecureRandom()
        {
            var random = new SecureRandom();
            return random;
        }

        /// <summary>
        /// Create a serial number used for certificate and other cryptography objects
        /// </summary>
        /// <param name="random">A crypto-random number generated</param>
        /// <returns>An integer value representing the serial number</returns>
        public BigInteger GenerateSerialNumber()
        {
            var random = new SecureRandom();
            return GenerateSerialNumber(random);
        }

        /// <summary>
        /// Get the certificate thumbprint/fingerprint
        /// </summary>
        /// <param name="certificate">The certificate for finding thumbprint</param>
        /// <returns>The SHA256 thumbprint of the certificate</returns>
        public string GetThumbprint(X509Certificate certificate)
        {
            byte[] certData = certificate.GetEncoded();

            var digest = new Sha256Digest();
            digest.BlockUpdate(certData, 0, certData.Length);
            byte[] digestedCert = new byte[digest.GetDigestSize()];
            digest.DoFinal(digestedCert, 0);
            byte[] hexBytes = Hex.Encode(digestedCert);

            return Encoding.ASCII.GetString(hexBytes);
        }

        /// <summary>
        /// Create a serial number used for certificate and other cryptography objects
        /// </summary>
        /// <param name="random">A crypto-random number generated</param>
        /// <returns>An integer value representing the serial number</returns>
        public BigInteger GenerateSerialNumber(SecureRandom random)
        {
            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            return serialNumber;
        }

        public string Hash(string content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            byte[] buffer = Encoding.Default.GetBytes(content);
            byte[] hashedBuffer = null;
            using (SHA256 s256 = SHA256.Create())
            {
                hashedBuffer = s256.ComputeHash(buffer);
            }

            var sb = new StringBuilder();
            for (int i = 0; i < hashedBuffer.Length; i++)
            {
                sb.Append($"{hashedBuffer[i]:X2}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Load a certificate from disk
        /// </summary>
        /// <param name="certPath"></param>
        /// <returns>An X.509 certificate or null if it is not on disk</returns>
        public async Task<X509Certificate> LoadCertificateAsync(string certPath)
        {
            X509Certificate cert = null;

            using (FileStream fs = File.OpenRead(certPath))
            {
                var buffer = new byte[fs.Length];
                int bytesRead = await fs.ReadAsync(buffer);

                //Ensure every byte read
                if (buffer.Length == bytesRead)
                {
                    var parser = new X509CertificateParser();
                    cert = parser.ReadCertificate(buffer);
                }
            }

            return cert;
        }

        public AsymmetricCipherKeyPair LoadKeyPair(string filePath, string encryptionKey, Algorithm algorithm)
        {
            using (FileStream fs = File.OpenRead(filePath))
            using (TextReader tr = new StreamReader(fs))
            {
                PemReader pr = new PemReader(tr, new PasswordFinder(encryptionKey));
                var pemObject = pr.ReadObject();

                if (algorithm == Algorithm.RSA)
                {
                    var privateKey = (RsaPrivateCrtKeyParameters)pemObject;
                    var publicKey = new RsaKeyParameters(false, privateKey.Modulus, privateKey.PublicExponent);
                    var keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);

                    return keyPair;
                }
                else if (algorithm == Algorithm.EllipticCurve)
                {
                    throw new NotImplementedException("Still figuring out how to read elliptic curve keys");
                }
            }

            throw new InvalidOperationException("This should never have occured in public AsymmetricCipherKeyPair LoadKeyPair(filePath, encryptionKey, algorithm)");
        }

        /// <summary>
        /// Save the certificate binary to disk
        /// </summary>
        /// <param name="certificate">The X.509 certificate</param>
        /// <param name="filePath">Path on disk of the </param>
        public async Task SaveCertificateAsync(X509Certificate certificate, string filePath)
        {
            using (FileStream fs = File.OpenWrite(filePath))
            {
                await fs.WriteAsync(certificate.GetEncoded());
            }
        }

        /// <summary>
        /// Save the public key to disk
        /// </summary>
        /// <param name="keyPair">The public key</param>
        /// <param name="filePath">Path to the public key file</param>
        /// <param name="encryptionRandom">A secure random for password encryption</param>
        /// <param name="pkPassword">Password to encrypt the public key</param>
        public void SavePrivateKey(AsymmetricCipherKeyPair keyPair, string filePath, SecureRandom encryptionRandom, string pkPassword=null)
        {
            var generator = new Pkcs8Generator(keyPair.Private, Pkcs8Generator.PbeSha1_3DES);

            generator.Password = string.IsNullOrEmpty(pkPassword) ? _config.ApplicationKey.ToCharArray() : pkPassword.ToCharArray();
            generator.SecureRandom = encryptionRandom;
            generator.IterationCount = 32; // TODO: Remove magic number

            var pemObject = generator.Generate();
            using (FileStream fs = File.OpenWrite(filePath))
            using (TextWriter tw = new StreamWriter(fs))
            {
                PemWriter pemWriter = new PemWriter(tw);
                pemWriter.WriteObject(pemObject);
                pemWriter.Writer.Flush();
            }
        }

        public byte[] Sign(string content, AsymmetricKey key)
        {
            throw new NotImplementedException("Coming soon!");
        }

        public ClaimsPrincipal ValidateJwt(string token, string issuer, string audience)
        {
            byte[] key = LoadEncryptionKey();
            var ssk = new SymmetricSecurityKey(key);
            SigningCredentials signingCredentials = new SigningCredentials(ssk, SecurityAlgorithms.HmacSha256Signature);

            var handler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,
                ValidateTokenReplay = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = audience,
                ValidIssuer = issuer,
                IssuerSigningKey = signingCredentials.Key
            };

            try
            {
                SecurityToken validatedToken;
                var claims = handler.ValidateToken(token, validationParams, out validatedToken);

                return claims;
            }
            catch (SecurityTokenInvalidAudienceException invalidAudience)
            {
                _log.Error(invalidAudience.Message);
            }
            catch (SecurityTokenInvalidIssuerException invalidIssuer)
            {
                _log.Error(invalidIssuer.Message);
            }
            catch (SecurityTokenInvalidSignatureException invalidSignature)
            {
                _log.Error(invalidSignature.Message);
            }
            catch (SecurityTokenReplayDetectedException replayDetected)
            {
                _log.Error(replayDetected.Message);
            }
            catch (SecurityTokenExpiredException expiredToken)
            {
                _log.Error(expiredToken.Message);
            }

            return null;
        }

        private byte[] LoadEncryptionKey()
        {
            byte[] encryptionKey = null;
            string path = $"{Environment.CurrentDirectory}/notary.key";
            if (File.Exists("notary.key"))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    encryptionKey = new byte[fs.Length];
                    int bytesRead = fs.Read(encryptionKey);
                }
            }
            else
            {
                using (RSA rsa = RSA.Create())
                {
                    encryptionKey = rsa.ExportRSAPrivateKey();

                    // Write the key to disk for future use.
                    using (FileStream fs = File.OpenWrite(path))
                    {
                        fs.Write(encryptionKey);
                    }
                }
            }

            return encryptionKey;
        }
    }
}
