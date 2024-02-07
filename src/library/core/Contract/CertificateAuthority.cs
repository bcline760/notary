using Notary.Model;

using System;
using System.Runtime.Serialization;

namespace Notary.Contract
{
    [DataContract]
    public class CertificateAuthority : Entity
    {
        public CertificateAuthority()
        {

        }

        public CertificateAuthority(CertificateAuthorityModel model) : base(model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.IssuingDn != null)
            {
                IssuingDn = new DistinguishedName
                {
                    CommonName = model.IssuingDn.CommonName,
                    Country = model.IssuingDn.Country,
                    Locale = model.IssuingDn.Locale,
                    Organization = model.IssuingDn.Organization,
                    OrganizationalUnit = model.IssuingDn.OrganizationalUnit,
                    StateProvince = model.IssuingDn.StateProvince
                };
            }

            DistinguishedName = new DistinguishedName
            {
                CommonName = model.DistinguishedName.CommonName,
                Country = model.DistinguishedName.Country,
                Locale = model.DistinguishedName.Locale,
                Organization = model.DistinguishedName.Organization,
                OrganizationalUnit = model.DistinguishedName.OrganizationalUnit,
                StateProvince = model.DistinguishedName.StateProvince
            };

            CertificateSlug = model.CertificateSlug;
            CrlEndpoint = model.CrlEndpoint;
            IsIssuer = model.IsIssuer;
            KeyAlgorithm = model.KeyAlgorithm;
            KeyCurve = model.KeyCurve;
            KeyLength = model.KeyLength;
            Name = model.Name;
            NotAfter = model.NotAfter;
            NotBefore = model.NotBefore;
            ParentCaSlug = model.ParentCaSlug;
        }

        /// <summary>
        /// Get or set slug of the certificate associated with the CA
        /// </summary>
        [DataMember]
        public string CertificateSlug { get; set; }

        /// <summary>
        /// Endpoint to the CRL
        /// </summary>
        [DataMember]
        public string CrlEndpoint { get; set; }

        /// <summary>
        /// Get or set the CA distinguished name
        /// </summary>
        [DataMember]
        public DistinguishedName DistinguishedName { get; set; }

        [DataMember]
        public bool IsIssuer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DistinguishedName IssuingDn { get; set; }

        /// <summary>
        /// Asymmetric key algorithm
        /// </summary>
        [DataMember]
        public Algorithm KeyAlgorithm
        {
            get; set;
        }

        /// <summary>
        /// The elliptic curve to use if EC is used to generate the keys
        /// </summary>
        [DataMember]
        public EllipticCurve? KeyCurve { get; set; }

        /// <summary>
        /// The length of the RSA key if RSA is used to generate the keys
        /// </summary>
        [DataMember]
        public int? KeyLength { get; set; }

        /// <summary>
        /// The name of the certificate authority
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The CA expiration
        /// </summary>
        [DataMember]
        public DateTime NotAfter { get; set; }

        /// <summary>
        /// The CA validity start
        /// </summary>
        [DataMember]
        public DateTime NotBefore { get; set; }

        /// <summary>
        /// The parent CA slug
        /// </summary>
        [DataMember]
        public string ParentCaSlug { get; set; }

        public override string[] SlugProperties()
        {
            return new string[] { Guid.NewGuid().ToString() };
        }
    }
}
