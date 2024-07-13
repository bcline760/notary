using Notary.Contract;
using Notary.Interface.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Service
{
    internal class LdapLoginService : LoginService
    {
        public LdapLoginService(IUserService userSvc) : base(userSvc)
        {
        }

        public override async Task<AuthenticationToken> Authenticate(ICredential credential)
        {
            throw new NotImplementedException();
        }
    }
}
