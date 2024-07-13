using Notary.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Interface.Service
{
    public interface ILoginService
    {
        /// <summary>
        /// Authenticate to the identity provider
        /// </summary>
        /// <param name="credential">The credenials with which to authenticate</param>
        /// <returns>An authentication token or null if bad credentials</returns>
        Task<AuthenticationToken> Authenticate(ICredential credential);
    }
}
