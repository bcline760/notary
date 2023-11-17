using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Notary.Contract;

using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Notary.Interface.Service
{
    public interface IEncryptionService
    {
        void AddAuthorityKeyIdentifier(X509V3CertificateGenerator certificateGenerator, X509Name issuerDN, AsymmetricCipherKeyPair issuerKeyPair, BigInteger issuerSerialNumber);
        void AddBasicConstraints(X509V3CertificateGenerator certificateGenerator, bool isCertificateAuthority);
        void AddExtendedKeyUsage(X509V3CertificateGenerator certificateGenerator, KeyPurposeID[] usages);
        void AddSubjectAlternativeNames(X509V3CertificateGenerator certificateGenerator, IEnumerable<SubjectAlternativeName> subjectAlternativeNames);
        void AddSubjectKeyIdentifier(X509V3CertificateGenerator certificateGenerator, AsymmetricCipherKeyPair subjectKeyPair);

        /// <summary>
        /// Generate a X.509 certificate
        /// </summary>
        /// <param name="sanList">List of subject alternative names</param>
        /// <param name="random">A cryptographic secure random number</param>
        /// <param name="alg">The key algorthm used</param>
        /// <param name="subjectDn">The certificate subject</param>
        /// <param name="subjectKeyPair">The certificate key</param>
        /// <param name="subjectSn">The certificate serial number</param>
        /// <param name="issuerDn">The issuing certificate</param>
        /// <param name="notAfter"></param>
        /// <param name="issuerKeyPair">The issuing certificate key</param>
        /// <param name="issuerSn">The issuing serial number</param>
        /// <param name="isCA">Boolean whether the certificate is a CA certificate</param>
        /// <param name="usages"></param>
        /// <returns></returns>
        X509Certificate GenerateCertificate(List<SubjectAlternativeName> sanList, SecureRandom random, Algorithm alg, string subjectDn, AsymmetricCipherKeyPair subjectKeyPair, BigInteger subjectSn, string issuerDn, DateTime notAfter, AsymmetricCipherKeyPair issuerKeyPair, BigInteger issuerSn, bool isCA, KeyPurposeID[] usages);

        /// <summary>
        /// Generate a JSON Web Token based on the given claims
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="tokenExpiry"></param>
        /// <returns></returns>
        string GenerateJwt(ClaimsIdentity claims, DateTime tokenExpiry);

        /// <summary>
        /// Generates an asymmetric key pair
        /// </summary>
        /// <param name="random"></param>
        /// <param name="algorithm">The algorithm to use</param>
        /// <param name="curve">If elliptic curve, this is the kind of curve to use</param>
        /// <param name="keyLength">If RSA, this is the length of the key</param>
        /// <returns>An asymmetric key pair</returns>
        AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, Algorithm algorithm, EllipticCurve? curve, int? keyLength);

        /// <summary>
        /// Create a serial number used for certificate and other cryptography objects
        /// </summary>
        /// <returns>An integer value representing the serial number</returns>
        BigInteger GenerateSerialNumber();

        /// <summary>
        /// Create a serial number used for certificate and other cryptography objects
        /// </summary>
        /// <param name="random">A crypto-random number generated</param>
        /// <returns>An integer value representing the serial number</returns>
        BigInteger GenerateSerialNumber(SecureRandom random);

        /// <summary>
        /// Get a cryptographically secure random number
        /// </summary>
        /// <returns>A random number suited for cryptography</returns>
        SecureRandom GetSecureRandom();

        /// <summary>
        /// Get the thumbprint from a X.509 certificate
        /// </summary>
        /// <param name="certificate">A X.509 certificate</param>
        /// <returns>The thumbprint belonging to X.509 certificate</returns>
        string GetThumbprint(X509Certificate certificate);

        /// <summary>
        /// Generate a cryptographically secure hash
        /// </summary>
        /// <param name="content">The content to hash</param>
        /// <returns>The hashed value using SHA-256</returns>
        string Hash(string content);

        /// <summary>
        /// Load a certificate from the filesystem
        /// </summary>
        /// <param name="certPath"></param>
        /// <returns></returns>
        Task<X509Certificate> LoadCertificateAsync(string certPath);

        /// <summary>
        /// Load the asymmetric key pair from the filesystem
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        AsymmetricCipherKeyPair LoadKeyPair(string filePath, string encryptionKey, Algorithm algorithm);

        /// <summary>
        /// Save the X.509 certificate to the file system
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task SaveCertificateAsync(X509Certificate certificate, string filePath);

        /// <summary>
        /// Save the encrypted private key to the filesystem
        /// </summary>
        /// <param name="keyPair">The private key to save</param>
        /// <param name="filePath">Path to the private key on the filesystem</param>
        /// <param name="encryptionRandom">Cryptographic random number for encryption</param>
        /// <param name="pkPassword">Private key password</param>
        void SavePrivateKey(AsymmetricCipherKeyPair keyPair, string filePath, SecureRandom encryptionRandom, string pkPassword=null);

        /// <summary>
        /// Validate a JWT agains the issuer and audience for authenticity
        /// </summary>
        /// <param name="token">The JWT to validate</param>
        /// <param name="issuer">The issuer to validate the JWT</param>
        /// <param name="audience">The audience of the JWT</param>
        /// <returns>A set of claims or null if the token is invalid</returns>
        ClaimsPrincipal ValidateJwt(string token, string issuer, string audience);
    }
}
