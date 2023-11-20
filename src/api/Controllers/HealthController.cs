using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notary.Api.Controllers
{
    [Route("api/[controller]")]
    public sealed class HealthController : NotaryController
    {
        public HealthController(ILog log) : base(log)
        {
        }

        [HttpGet, Route("token")]
        public async Task<IActionResult> CheckTokenAsync()
        {
            return Ok("Token valid");
        }

        [HttpGet, Route("check"), AllowAnonymous]
        public async Task<IActionResult> Check()
        {
            return Ok("OK");
        }
    }
}