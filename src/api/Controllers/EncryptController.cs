﻿using log4net;

using Microsoft.AspNetCore.Mvc;

using Notary.Configuration;
using Notary.Interface.Service;

namespace Notary.Api.Controllers
{
    public class EncryptController : NotaryController
    {
        public EncryptController(IEncryptionService encService, NotaryConfiguration config, ILog log) : base(log)
        {
            Service = encService;
        }

        protected IEncryptionService Service { get; set; }
    }
}