using log4net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace Notary.Api.Controllers
{
    [Authorize(Roles = "Notary.Admin")]
    public class AdminController : NotaryController
    {
        public AdminController(ILog log) : base(log)
        {
        }
    }
}
