using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Notary
{
    /// <summary>
    /// Defines the roles of an application
    /// </summary>
    [DataContract]
    public enum Roles
    {
        /// <summary>
        /// User has permissions to all features of the application, access to all keys
        /// </summary>
        [EnumMember]
        Admin,
        /// <summary>
        /// User has permissions to manage certificates
        /// </summary>
        [EnumMember]
        CertificateAdmin,
        /// <summary>
        /// User has has basic permissions to download certificates and perform encryption
        /// </summary>
        [EnumMember]
        User
    }
}
