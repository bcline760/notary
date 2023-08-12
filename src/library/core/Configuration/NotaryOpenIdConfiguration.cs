using System;
using System.Collections.Generic;
using System.Text;

namespace Notary.Configuration
{
    public class NotaryOpenIdConfiguration
    {
        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string MetadataAddress { get; set; }
    }
}
