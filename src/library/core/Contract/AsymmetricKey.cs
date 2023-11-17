using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Notary.Contract
{
    public sealed class AsymmetricKey : Key
    {
        public AsymmetricKey() { }

        public AsymmetricKey(byte[] publicKey, byte[] privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        [JsonProperty("pub_key", Required = Required.Always)]
        public byte[] PublicKey { get; set; }

        [JsonProperty("prv_key", Required = Required.Always)]
        public byte[] PrivateKey { get; set; }
    }
}
