using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Contract
{
    public sealed class AsymmetricKey : Key
    {
        public AsymmetricKey() { }

        public AsymmetricKey(byte[] publicKey, byte[] privateKey)
        {

        }

        public byte[] PublicKey { get; set; }

        public byte[] PrivateKey { get; set; }
    }
}
