using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace Notary.Contract
{
    [DataContract]
    public sealed class CertificateAuthoritySetup
    {
        [DataMember]
        public Algorithm Algorithm { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public EllipticCurve? Curve { get; set; }

        [DataMember]
        public bool IsIssuer { get; set; }

        [DataMember]
        public int? KeyLength { get; set; }

        [DataMember]
        public int LengthInYears { get; set; }

        [DataMember]
        public string Locale { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ParentCaSlug { get; set; }

        [DataMember]
        public string Organization { get; set; }

        [DataMember]
        public string OrganizationalUnit { get; set; }

        [DataMember]
        public string Requestor { get; set; }

        [DataMember]
        public string StateProvince { get; set; }
    }
}
