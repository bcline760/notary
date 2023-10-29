using Newtonsoft.Json;

namespace Notary.Configuration
{
    /// <summary>
    /// Defines the configuration used for this application
    /// </summary>
    public class NotaryConfiguration
    {
        public NotaryConfiguration()
        {

        }

        public NotaryConfiguration(NotaryConfiguration config)
        {
            ApplicationKey = config.ApplicationKey;
            ActiveDirectory = config.ActiveDirectory;
            Authentication = config.Authentication;
            Database = config.Database;
            HashLength = config.HashLength;
            TokenSettings = config.TokenSettings;
        }

        public NotaryActiveDirectoryConfiguration ActiveDirectory { get; set; }

        public string ApplicationKey { get; set; }

        public AuthenticationProvider Authentication { get; set; }

        public NotaryDatabaseConfiguration Database { get; set; }

        public int HashLength { get; set; }

        public string RootDirectory { get; set; }

        public NotaryTokenSettingsConfiguration TokenSettings { get; set; }
    }
}
