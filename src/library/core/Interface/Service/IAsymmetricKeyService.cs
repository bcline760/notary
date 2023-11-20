using Notary.Contract;
using Notary.Interface.Service;

using Org.BouncyCastle.Crypto;

using System.Threading.Tasks;

namespace Notary.Service
{
    public interface IAsymmetricKeyService : IEntityService<AsymmetricKey>
    {
        /// <summary>
        /// Get a public/private key combination
        /// </summary>
        /// <param name="slug">The key slub</param>
        /// <returns></returns>
        Task<AsymmetricCipherKeyPair> GetKeyPairAsync(string slug);

        /// <summary>
        /// Get a public key from the private key
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        Task<byte[]> GetPublicKey(string slug);
    }
}