using log4net;

using Notary.Contract;
using Notary.Interface.Repository;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Service
{
    public abstract class CryptographicEntityService<TC> : EntityService<TC>
        where TC : Entity, new()
    {
        public CryptographicEntityService(IRepository<TC> repository, ILog log) : base(repository, log)
        {
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
    }
}
