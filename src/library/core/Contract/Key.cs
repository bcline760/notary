using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Contract
{
    [KnownType(typeof(SymmetricKey))]
    [KnownType(typeof(AsymmetricKey))]
    public abstract class Key : Entity
    {
        protected Key() { }

        public override string[] SlugProperties()
        {
            throw new NotImplementedException();
        }

        [JsonProperty("alg", Required = Required.Always)]
        public Algorithm KeyAlgorithm { get; set; }

        [JsonProperty("nb", DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.Always)]
        public DateTime NotBefore { get; set; }

        [JsonProperty("na", DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.Always)]
        public DateTime NotAfter { get; set; }

    }
}
