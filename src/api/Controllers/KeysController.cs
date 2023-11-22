using System.Net;
using System.Runtime.CompilerServices;
using log4net;
using Microsoft.AspNetCore.Mvc;

using Notary.Logging;
using Notary.Api.Controllers;
using Notary.Contract;
using Notary.Interface.Service;

namespace Notary.Api;

public class KeysController : NotaryController
{
    public KeysController(IAsymmetricKeyService service, ILog log) : base(log)
    {
        KeyService = service;
    }

    [Route("{slug}/public"), HttpGet]
    public async Task<IActionResult> GetPublicKey(string slug)
    {
        IActionResult result = null;
        try
        {
            var key = await KeyService.GetPublicKey(slug);
            result = key == null ? NotFound() : Ok(key);
        }
        catch (Exception ex)
        {
            ex.IfNotLoggedThenLog(Log);
            result = StatusCode((int)HttpStatusCode.InternalServerError);
        }

        return result;
    }

    protected IAsymmetricKeyService KeyService { get; }
}
