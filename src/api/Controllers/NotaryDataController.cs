using System.Net;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Notary.Contract;
using Notary.Interface.Service;
using Notary.Logging;

namespace Notary.Api.Controllers
{
    public abstract class NotaryDataController<T> : NotaryController
        where T : Entity
    {
        protected IEntityService<T> Service { get; private set; }
        protected NotaryDataController(IEntityService<T> service, ILog log) : base(log)
        {
            Service = service;
        }

        [HttpGet, Route("")]
        public virtual async Task<IActionResult> GetAllAsync()
        {
            var result = await ExecuteServiceMethod(Service.GetAllAsync);

            return result;
        }

        [HttpGet, Route("{slug}")]
        public virtual async Task<IActionResult> GetAsync(string slug)
        {
            var result = await ExecuteServiceMethod(Service.GetAsync, slug);

            return result;
        }

        [HttpPost, Route("")]
        public virtual async Task<IActionResult> PostAsync([FromBody] T contract)
        {
            try
            {

                await Service.SaveAsync(contract, "system");
                return Created(Request.Path.ToUriComponent(), contract);
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut, Route("{slug}")]
        public virtual async Task<IActionResult> PutAsync(string slug, [FromBody] T oldContract)
        {
            try
            {
                var contract = await Service.GetAsync(slug);
                if (contract == null)
                    return new NotFoundResult();

                contract = oldContract;

                await Service.SaveAsync(contract, "system");
                return new OkResult();
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpDelete, Route("{slug}")]
        public virtual async Task<IActionResult> DeleteAsync(string slug)
        {
            var slugClaim = User.Claims.FirstOrDefault(c => c.Type == "slug");

            try
            {
                var contract = await Service.GetAsync(slug);
                if (contract != null)
                {
                    contract.Active = false;
                    contract.Updated = DateTime.Now;

                    await Service.SaveAsync(contract, slugClaim == null ? slugClaim?.Value : "nobody");
                    return Ok();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                ex.IfNotLoggedThenLog(Log);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
