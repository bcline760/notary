using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notary.Contract;
using Notary.Interface.Service;

namespace Notary.Api.Controllers
{
    [Route("api/ca")]
    [ApiController]
    public class CertificateAuthorityController : NotaryDataController<CertificateAuthority>
    {
        public CertificateAuthorityController(ICertificateAuthorityService service, ICertificateService certSerivce, ILog log) : base(service, log)
        {
            CertificateService = certSerivce;
        }

        //[Route("{slug}/certificate/{thumbprint}"), HttpGet]
        //public async Task<IActionResult> GetCertificateAuthorityCertificate(string slug, string thumbprint)
        //{
        //    var service = (ICertificateAuthorityService)Service;

        //    var ca = await service.GetAsync(slug);
        //}

        [Route("{slug}/certificates"), HttpGet]
        public async Task<IActionResult> GetCertificates(string slug)
        {
            var service = (ICertificateAuthorityService)Service;

            var result = await ExecuteServiceMethod(CertificateService.GetCertificatesByCaAsync, slug);

            return result;
        }

        [Route("calist"), HttpGet]
        public async Task<IActionResult> GetCaListBreif()
        {
            var service = (ICertificateAuthorityService)Service;

            var result = await ExecuteServiceMethod(service.GetCaListBrief);

            return result;
        }

        // [Authorize(Roles ="Notary.Admin")]
        [Route("setup"), HttpPost]
        public async Task<IActionResult> SetupCertificateAuthority(CertificateAuthoritySetup setup)
        {
            var service = (ICertificateAuthorityService)Service;

            // if (!User.HasClaim(c => c.Type == "oid"))
            // {
            //     return BadRequest();
            // }

            // var userId = User.Claims.First(c => c.Type == "oid").Value;
            setup.Requestor = "system";

            var result = await ExecuteServiceMethod(service.SetupCertificateAuthority, setup, true);

            return result;
        }

        public ICertificateService CertificateService { get; }
    }
}
