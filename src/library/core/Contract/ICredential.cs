using System;
using System.Collections.Generic;
using System.Text;

namespace Notary.Contract
{
    public interface ICredential
    {
        /// <summary>
        /// Get the key value used in credentialling. This can be a username or a service principal account name.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Get or secret value that is associated with the secret. This can be a password, a token string, or part of a service principal
        /// </summary>
        string Secret { get; }

        /// <summary>
        /// Get when the credential will expire.
        /// </summary>
        bool Persistant { get; }
    }
}
