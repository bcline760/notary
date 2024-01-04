using Notary.Contract;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Notary.Web.ViewModels
{
    public class CreateCertificateAuthorityViewModel
    {
        public CreateCertificateAuthorityViewModel()
        {
            Country = string.Empty;
            Locale = string.Empty;
            Name = string.Empty;
            ParentCaSlug = string.Empty;
            Organization = string.Empty;
            OrganizationalUnit = string.Empty;
            StateProvince = string.Empty;
        }

        [Required]
        public Algorithm Algorithm { get; set; }

        public string Country { get; set; }

        public EllipticCurve? Curve { get; set; }

        [Required]
        public bool IsIssuer { get; set; }

        public int? KeyLength { get; set; }

        [Required, Range(5, 10, ErrorMessage = "Please enter a valid length of 5-10 years")]
        public int LengthInYears { get; set; }

        public string Locale { get; set; }

        [Required, RegularExpression("[a-zA-Z0-9\\s]+", ErrorMessage = "Only alphanumerics plus spaces allowed")]
        public string Name { get; set; }

        public string ParentCaSlug { get; set; }

        public string Organization { get; set; }

        public string OrganizationalUnit { get; set; }

        public string StateProvince { get; set; }

        public List<CertificateAuthority>? CertificateAuthorities { get; set; }

        public CertificateAuthority? SelectedCa { get; set; }
    }
}
