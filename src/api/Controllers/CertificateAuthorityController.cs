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

        [Route("{slug}/certificates"), HttpGet]
        public async Task<IActionResult> GetCertificates(string slug)
        {
            var service = (ICertificateAuthorityService)Service;

            var result = await ExecuteServiceMethod(CertificateService.GetCertificatesByCaAsync, slug);

            return result;
        }

        [Route("calist"), HttpGet]
        public async Task<IActionResult> GetCaListBrief()
        {
            var service = (ICertificateAuthorityService)Service;

            var result = await ExecuteServiceMethod(service.GetCaListBrief);

            return result;
        }

        public ICertificateService CertificateService { get; }
    }
}
