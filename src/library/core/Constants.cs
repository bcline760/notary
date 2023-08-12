using System;
using System.Collections.Generic;
using System.Text;

namespace Notary
{
    public static class Constants
    {
        public static readonly string AppKeyEnv = "NOTARY_APP_KEY";

        public static readonly string AdAdminGroupNameEv = "NOTARY_AD_ADMIN_GROUP_NAME";
        public static readonly string AdCertAdminGroupNameEv = "NOTARY_AD_CERT_ADMIN_GROUP_NAME";
        public static readonly string AdDomainEv = "NOTARY_AD_DOMAIN";
        public static readonly string AdServerNameEv = "NOTARY_AD_SERVER_NAME";
        public static readonly string AdLoginEv = "NOTARY_AD_LOGIN";
        public static readonly string AdPasswordEv = "NOTARY_AD_PASSWORD";

        public static readonly string DatabaseConnectiongStringEv = "NOTARY_DB_CONN_STR";
        public static readonly string DatabaseLoginEv = "NOTARY_DB_LOGIN";
        public static readonly string DatabaseNameEv = "NOTARY_DB_NAME";
        public static readonly string DatabasePasswordEv = "NOTARY_DB_PASSWORD";
        public static readonly string CertificateDirectoryPath = "certificates";
        public static readonly string KeyDirectoryPath = "keys";

        public static readonly string RoleAdmin = "Notary.Admin";
        public static readonly string RoleCertificateAdmin = "Notary.CertificateAdmin";
        public static readonly string RoleUser = "Notary.User";
    }
}
