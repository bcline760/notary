using log4net;

using Notary.Configuration;
using Notary.Contract;
using Notary.Interface.Service;

using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Notary.Api.Controllers
{
    public class LoginController : NotaryController
    {
        public LoginController(ILog log, ILoginService service, NotaryConfiguration config) : base(log)
        {
            LoginService = service;
            Configuration = config;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]ICredential credential)
        {
            var result = await ExecuteServiceMethod(LoginService.Authenticate, credential);

            return result;
        }

        protected ILoginService LoginService { get; }

        protected NotaryConfiguration Configuration { get; }
    }
}
