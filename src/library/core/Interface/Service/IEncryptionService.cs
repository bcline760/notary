using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Notary.Contract;


using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Notary.Interface.Service
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Generate a JSON Web Token based on the given claims
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="tokenExpiry"></param>
        /// <returns></returns>
        string GenerateJwt(ClaimsIdentity claims, DateTime tokenExpiry);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strength"></param>
        /// <returns></returns>
        AsymmetricCipherKeyPair GenerateKeyPair(int strength);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, int strength);

        /// <summary>
        /// Generate a secure hash of a plaintext password using PBKDF
        /// </summary>
        /// <param name="plainText">The incoming</param>
        /// <returns>The PBKDF hashed password</returns>
        byte[] GeneratePasswordHash(string plainText);

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
        /// Generate a cryptographically secure hash
        /// </summary>
        /// <param name="content">The content to hash</param>
        /// <returns>The hashed value using SHA-256</returns>
        string Hash(string content);

        /// <summary>
        /// Load the asymmetric key pair from the filesystem
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encryptionKey"></param>
        /// <returns></returns>
        AsymmetricCipherKeyPair LoadKeyPair(string filePath, string encryptionKey);

        /// <summary>
        /// Save the private key to disk
        /// </summary>
        /// <param name="keyPair">The public/private in which the private part will be saved to disk</param>
        /// <param name="filePath"></param>
        /// <param name="encryptionRandom"></param>
        void SavePrivateKey(AsymmetricCipherKeyPair keyPair, string filePath, SecureRandom encryptionRandom);

        /// <summary>
        /// Validate a JWT agains the issuer and audience for authenticity
        /// </summary>
        /// <param name="token">The JWT to validate</param>
        /// <param name="issuer">The issuer to validate the JWT</param>
        /// <param name="audience">The audience of the JWT</param>
        /// <returns>A set of claims or null if the token is invalid</returns>
        ClaimsPrincipal ValidateJwt(string token, string issuer, string audience);

        /// <summary>
        /// Verify the password to ash to see if it is a valid hash.
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        bool VerifyPasswordHash(byte[] passwordHash);
    }
}
