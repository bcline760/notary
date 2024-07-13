using Notary.Contract;
using Notary.Interface.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Service
{
    internal abstract class LoginService : ILoginService
    {
        protected LoginService(IUserService userSvc) { 
            UserService = userSvc;
        }

        public abstract Task<AuthenticationToken> Authenticate(ICredential credential);

        /// <summary>
        /// Get a reference to the user service to read & write users
        /// </summary>
        protected IUserService UserService { get; }
    }
}
