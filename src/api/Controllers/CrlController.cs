using log4net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Notary.Contract;
using Notary.Interface.Service;

namespace Notary.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class CrlController : NotaryDataController<RevocatedCertificate>
    {
        public CrlController(ICertificateRevokeService service, ILog log) : base(service, log)
        {
        }

        [HttpGet, Route("list"), AllowAnonymous]
        public async Task<IActionResult> GetCrlAsync(string slug)
        {
            var service = (ICertificateRevokeService)Service;

            var result = await ExecuteServiceMethod(service.GenerateCrl, slug);

            return result;
        }
    }
}
