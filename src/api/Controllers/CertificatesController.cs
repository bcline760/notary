using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notary.Contract;
using Notary.Interface.Service;

namespace Notary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : NotaryDataController<Certificate>
    {
        public CertificatesController(ICertificateService certService, ILog log) : base(certService, log)
        {
        }

        [HttpPost, Route("{slug}/download-request")]
        public async Task<IActionResult> RequestDownloadCertificateAsync([FromBody] CertificateDownloadRequest request)
        {
            if (request.Format == CertificateFormat.Pkcs12 && string.IsNullOrEmpty(request.Password))
                return BadRequest("Please supply a private key password for PKCS #12 certificates");

            var service = (ICertificateService)Service;
            var cert = await service.GetAsync(request.Slug);

            if (cert != null)
            {
                var certBinary = await service.RequestCertificateAsync(request.Slug, request.Format, request.Password);

                return File(certBinary, "application/octet-stream");
            }

            return NotFound();
        }

        // [Authorize(Roles ="Notary.Admin,Notary.CertificateAdmin,Notary.User")]
        [HttpPost, Route("issue")]
        public async Task<IActionResult> IssueCertificateAsync([FromBody] CertificateRequest request)
        {
            if (request == null)
                return BadRequest();

            var service = (ICertificateService)Service;

            var result = await ExecuteServiceMethod(service.IssueCertificateAsync, request);

            return result;
        }

        // [Authorize(Roles ="Notary.Admin,Notary.CertificateAdmin,Notary.User")]
        [HttpDelete, Route("{slug}")]
        public override async Task<IActionResult> DeleteAsync(string slug)
        {
            IActionResult result = null;
            var service = (ICertificateService)Service;
            var certificate = await service.GetAsync(slug);

            if (certificate == null)
                result = NotFound();
            else
            {
                if (certificate.IsCaCertificate)
                    result = Forbid();
                else
                {
                    certificate.Active = false;
                    certificate.RevocationDate = DateTime.UtcNow;
                    certificate.Updated = DateTime.UtcNow;

                    await service.SaveAsync(certificate, null);
                    result = Ok();
                }
            }
            return result;
        }
    }
}