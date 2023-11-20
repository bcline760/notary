using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace Notary.Contract
{
    [DataContract]
    public class CertificateRequest
    {
        public CertificateRequest()
        {
        }

        [DataMember]
        public string CertificatePassword { get; set; }

        [DataMember]
        public EllipticCurve? Curve { get; set; }

        [DataMember]
        public Algorithm KeyAlgorithm { get; set; }

        [DataMember]
        public int? KeySize { get; set; }

        [DataMember]
        public short KeyUsage { get; set; }

        /// <summary>
        /// Get or set the expiration length in hours.
        /// </summary>
        [DataMember]
        public int LengthInHours { get; set; }

        /// <summary>
        /// Get or set the display name of the certificate
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ParentCertificateSlug { get; set; }

        [DataMember]
        public DistinguishedName Subject { get; set; }

        /// <summary>
        /// Get a list of SAN for the certificate
        /// </summary>
        [DataMember]
        public List<SubjectAlternativeName> SubjectAlternativeNames
        {
            get; set;
        }

        /// <summary>
        /// Get or set the account slug that requested this certificate
        /// </summary>
        [DataMember]
        public string RequestedBySlug { get; set; }
    }
}
