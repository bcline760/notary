using System;
using System.Collections.Generic;
using System.Text;

namespace Notary.Configuration
{
    public class NotaryDatabaseConfiguration
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
