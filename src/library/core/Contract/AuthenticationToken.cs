using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Contract
{
    public class AuthenticationToken : Entity
    {
        public string AccessToken { get; set; }

        public DateTime Expiry { get; set; }

        public string UserName { get; set; } = string.Empty;

        public override string[] SlugProperties() => new string[] { Guid.NewGuid().ToString() };
    }
}
