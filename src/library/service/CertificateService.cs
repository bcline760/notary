using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using log4net;

using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

using Notary.Configuration;
using Notary.Contract;
using Notary.Interface.Repository;
using Notary.Interface.Service;
using Notary.Logging;


namespace Notary.Service
{
    public class CertificateService : EntityService<Certificate>, ICertificateService
    {
        public CertificateService(
            NotaryConfiguration config,
            Interface.Repository.ICertificateRepository repository,
            ICertificateAuthorityService certificateAuthorityService,
            ILog log) : base(repository, log)
        {
            Configuration = config;
            CertificateAuthority = certificateAuthorityService;
        }

        public async Task<Certificate> IssueCertificateAsync(CertificateRequest request)
        {
            try
            {
                var certificateAuthority = await CertificateAuthority.GetAsync(request.CertificateAuthoritySlug);
                if (certificateAuthority == null)
                    throw new InvalidOperationException($"Unable to find certificate authority with GUID {request.CertificateAuthoritySlug}");
                if (!certificateAuthority.IsIssuer)
                    throw new InvalidOperationException("This certificate authority is not an issuer of certificates");

                string path = $"{Configuration.RootDirectory}/{certificateAuthority.Slug}/";

                //Load the issuer's private key from the file system.
                var keyPath = $"{path}/{Constants.KeyDirectoryPath}/{certificateAuthority.IssuingThumbprint}.key.pem";
                var issuerKeyPair = CertificateMethods.LoadKeyPair(keyPath, Configuration.ApplicationKey);

                var issuerSn = new BigInteger(certificateAuthority.IssuingSerialNumber, 16);
                var random = CertificateMethods.GetSecureRandom();
                var certificateKeyPair = CertificateMethods.GenerateKeyPair(random, request.KeyAlgorithm, request.Curve, request.KeySize);
                var serialNumber = CertificateMethods.GenerateSerialNumber(random);
                var subject = DistinguishedName.BuildDistinguishedName(request.Subject);
                var notAfter = DateTime.UtcNow.AddHours(request.LengthInHours);
                var issuerDn = DistinguishedName.BuildDistinguishedName(certificateAuthority.DistinguishedName);
                var keyUsages = new List<KeyPurposeID>();

                //TODO: There has to be a better way to do this...
                if ((request.KeyUsage & (int)KeyPurposeFlags.ClientAuthentication) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPClientAuth);
                if ((request.KeyUsage & (int)KeyPurposeFlags.CodeSigning) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPCodeSigning);
                if ((request.KeyUsage & (int)KeyPurposeFlags.EmailProtection) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPEmailProtection);
                if ((request.KeyUsage & (int)KeyPurposeFlags.IpsecEndSystem) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPIpsecEndSystem);
                if ((request.KeyUsage & (int)KeyPurposeFlags.IpsecTunnel) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPIpsecTunnel);
                if ((request.KeyUsage & (int)KeyPurposeFlags.IpsecUser) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPIpsecUser);
                if ((request.KeyUsage & (int)KeyPurposeFlags.MacAddress) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPMacAddress);
                if ((request.KeyUsage & (int)KeyPurposeFlags.OcspSigning) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPOcspSigning);
                if ((request.KeyUsage & (int)KeyPurposeFlags.ServerAuthentication) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPServerAuth);
                if ((request.KeyUsage & (int)KeyPurposeFlags.SmartCardLogon) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPSmartCardLogon);
                if ((request.KeyUsage & (int)KeyPurposeFlags.TimeStamping) != 0)
                    keyUsages.Add(KeyPurposeID.IdKPTimeStamping);

                //Generate the certificate
                var generatedCertificate = CertificateMethods.GenerateCertificate(
                    request.SubjectAlternativeNames,
                    random,
                    request.KeyAlgorithm,
                    subject,
                    certificateKeyPair,
                    serialNumber,
                    issuerDn,
                    notAfter,
                    issuerKeyPair,
                    issuerSn,
                    false,
                    keyUsages.ToArray()
                );

                var thumbprint = CertificateMethods.GetThumbprint(generatedCertificate);
                var issuedKeyPath = $"{path}/{Constants.KeyDirectoryPath}/{thumbprint}.key.pem";
                var issuedCertPath = $"{path}/{Constants.CertificateDirectoryPath}/{thumbprint}.cer";

                var certificate = new Certificate
                {
                    Active = true,
                    KeyAlgorithm = request.KeyAlgorithm,
                    CertificateAuthoritySlug = certificateAuthority.Slug,
                    Created = DateTime.UtcNow,
                    CreatedBySlug = request.RequestedBySlug,
                    Issuer = certificateAuthority.DistinguishedName,
                    KeyUsage = request.KeyUsage,
                    Name = request.Name,
                    NotAfter = notAfter,
                    NotBefore = DateTime.UtcNow,
                    SerialNumber = generatedCertificate.SerialNumber.ToString(16),
                    Subject = request.Subject,
                    SignatureAlgorithm = generatedCertificate.SigAlgName,
                    SubjectAlternativeNames = request.SubjectAlternativeNames,
                    Thumbprint = thumbprint
                };

                if (request.Curve.HasValue)
                    certificate.KeyCurve = request.Curve.Value;

                await SaveAsync(certificate, request.RequestedBySlug);

                //Save encrypted private key and certificate to the file system
                CertificateMethods.SavePrivateKey(certificateKeyPair, issuedKeyPath, random, Configuration.ApplicationKey);
                CertificateMethods.SaveCertificate(generatedCertificate, issuedCertPath);

                return certificate;
            }
            catch (Exception ex)
            {
                throw ex.IfNotLoggedThenLog(Logger);
            }
        }

        public async Task<byte[]> RequestCertificateAsync(string slug, CertificateFormat format, string privateKeyPassword)
        {
            var certificate = await GetAsync(slug);
            if (certificate == null)
                return null;

            var path = $"{Configuration.RootDirectory}/{certificate.CertificateAuthoritySlug}/";

            byte[] certificateBinary = null;

            string certificatePath = $"{path}/{Constants.CertificateDirectoryPath}/{certificate.Thumbprint}.cer";
            X509Certificate cert = await CertificateMethods.LoadCertificate(certificatePath);
            if (cert == null)
                return null;
            cert.CheckValidity();

            switch (format)
            {
                case CertificateFormat.Der:
                    certificateBinary = cert.GetEncoded();
                    break;
                case CertificateFormat.Pem:
                    using (var stream = new MemoryStream())
                    {
                        using (TextWriter tw = new StreamWriter(stream))
                        {
                            var pw = new PemWriter(tw);
                            pw.WriteObject(cert);
                            await pw.Writer.FlushAsync();
                        }

                        certificateBinary = stream.ToArray();
                    }
                    break;
                case CertificateFormat.Pkcs12:
                    var certKeyPath = $"{path}/{Constants.KeyDirectoryPath}/{certificate.Thumbprint}.key.pem";
                    var certKey = CertificateMethods.LoadKeyPair(certKeyPath, Configuration.ApplicationKey);
                    var store = new Pkcs12StoreBuilder().Build();
                    var certEntry = new X509CertificateEntry(cert);
                    var keyEntry = new AsymmetricKeyEntry(certKey.Private);

                    store.SetKeyEntry(certificate.Subject.ToString(), keyEntry, new X509CertificateEntry[] { certEntry });
                    using (var memStream = new MemoryStream())
                    {
                        store.Save(memStream, privateKeyPassword.ToArray(), CertificateMethods.GetSecureRandom());
                        certificateBinary = memStream.ToArray();
                    }
                    break;
                case CertificateFormat.Pkcs7:
                    //TODO: Figure this out.
                    break;
                default:
                    throw new ArgumentException(nameof(format));
            }
            return certificateBinary;
        }

        public async Task<List<Certificate>> GetCertificatesByCaAsync(string slug)
        {
            var repository = (Interface.Repository.ICertificateRepository)Repository;

            return await repository.GetCertificatesByCaAsync(slug);
        }

        protected NotaryConfiguration Configuration { get; }

        protected ICertificateAuthorityService CertificateAuthority { get; }
    }
}
