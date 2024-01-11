using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
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
        public EllipticCurve? Curve { get; set; }

        [DataMember, Required]
        public bool IsCaCertificate { get; set; }

        [DataMember, Required]
        public Algorithm KeyAlgorithm { get; set; }

        [DataMember]
        public int? KeySize { get; set; }

        [DataMember]
        public List<string> KeyUsages { get; set; }

        /// <summary>
        /// Get or set the display name of the certificate
        /// </summary>
        [DataMember, Required, RegularExpression("[a-zA-Z0-9\\s]+", ErrorMessage = "Only alphanumerics plus spaces allowed")]
        public string Name { get; set; }

        [DataMember]
        public DateTime NotAfter { get; set; }

        [DataMember]
        public DateTime NotBefore { get; set; }

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
