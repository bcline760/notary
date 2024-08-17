using Notary.Contract;

namespace Notary.Web.ViewModels
{
    public class CertificateViewModel
    {
        public CertificateViewModel()
        {
            SerialNumber = string.Empty;
            SignatureAlgorithm = string.Empty;
            Thumbprint = string.Empty;
        }

        public EllipticCurve? EllipticCurve { get; set; }

        public bool Expired { get;set; }

        public bool Expiring { get; set; }

        public DistinguishedName Issuer
        {
            get; set;
        }

        public Algorithm KeyAlgorithm { get; set; }

        public List<string> KeyUsages
        {
            get; set;
        }

        public string Name { get; set; }

        public DateTime NotAfter
        {
            get; set;
        }

        public DateTime NotBefore
        {
            get; set;
        }

        /// <summary>
        /// Get or set the date the certificate was issued
        /// </summary>
        public DateTime? RevocationDate
        {
            get; set;
        }

        public string RevocationReason { get; set; }

        public int? RsaKeyLength { get; set; }

        public string SerialNumber
        {
            get; set;
        }

        public string SignatureAlgorithm { get; set; }

        public DistinguishedName Subject { get; set; }

        public List<SubjectAlternativeName> SubjectAlternativeNames
        {
            get; set;
        }

        public string Thumbprint
        {
            get; set;
        }
    }
}
