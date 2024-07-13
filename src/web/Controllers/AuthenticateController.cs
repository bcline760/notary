using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Notary.Contract;

namespace Notary.Web.Controllers
{
    [Route("[controller]")]
    public class AuthenticateController : Controller
    {
        [Route("Login"), HttpPost]
        public async Task Login(string returnUrl="/")
        {
            //TODO: Implement Open ID
        }

        [Authorize, Route("Logout"), HttpGet]
        public async Task Logout()
        {
            // Clear auth cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
