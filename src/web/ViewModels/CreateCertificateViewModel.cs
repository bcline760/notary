using Notary.Contract;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Notary.Web.ViewModels
{
    public class CreateCertificateViewModel
    {
        public CreateCertificateViewModel()
        {
            CertificateAuthoritySlug = string.Empty;
            SelectedKeyUsages = new List<string>();
            Name = string.Empty;
            Subject = new DistinguishedName();
            SubjectAlternativeNames = new List<SubjectAlternativeName>();

            KeyUsages = new Dictionary<string, string>
            {
                {"1.3.6.1.5.5.7.3.1","Server Authentication" },
                {"1.3.6.1.5.5.7.3.2","Client Authentication" },
                {"1.3.6.1.5.5.7.3.3","Code Signing" },
                {"1.3.6.1.5.5.7.3.4","E-mail Protection" },
                {"1.3.6.1.5.5.7.3.5","IPSEC End System" },
                {"1.3.6.1.5.5.7.3.6","IPSEC Tunnel" },
                {"1.3.6.1.5.5.7.3.7","IPSEC User" },
                {"1.3.6.1.5.5.7.3.8","Time Stamping" },
                {"1.3.6.1.5.5.7.3.9","OCSP Signing" },
                {"1.3.6.1.4.1.311.20.2.2","SMART Card Login"},
                {"1.3.6.1.1.1.1.22","MAC Address"}
            };
        }

        public string CertificateAuthoritySlug { get; set; }

        public EllipticCurve? Curve { get; set; }

        /// <summary>
        /// Get or set the expiration length in hours.
        /// </summary>
        [Required, Range(1, 10, ErrorMessage = "Must be from 1 to 10 years")]
        public int ExpiryLength { get; set; }

        [Required]
        public Algorithm KeyAlgorithm { get; set; }

        [Range(2048, 8192, ErrorMessage = "Must have a key size between 2048 and 8192 bits")]
        public int? KeySize { get; set; }

        public Dictionary<string, string> KeyUsages { get; }

        public List<string> SelectedKeyUsages { get; }

        /// <summary>
        /// Get or set the display name of the certificate
        /// </summary>
        [Required, RegularExpression("[a-zA-Z0-9\\s]+", ErrorMessage = "Only alphanumerics plus spaces allowed")]
        public string Name { get; set; }

        public CertificateAuthority? SelectedCa { get; set; }

        public DistinguishedName Subject { get; }

        /// <summary>
        /// Get a list of SAN for the certificate
        /// </summary>
        public List<SubjectAlternativeName> SubjectAlternativeNames
        {
            get;
        }
    }
}
