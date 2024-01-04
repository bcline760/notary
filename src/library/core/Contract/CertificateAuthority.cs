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
            IsIssuer = model.IsIssuer;
            KeyAlgorithm = model.KeyAlgorithm;
            KeyCurve = model.KeyCurve;
            KeyLength = model.KeyLength;
            Name = model.Name;
            ParentCaSlug = model.ParentCaSlug;
        }

        /// <summary>
        /// Get or set slug of the certificate associated with the CA
        /// </summary>
        [DataMember]
        public string CertificateSlug { get; set; }

        /// <summary>
        /// Get or set the CA distinguished name
        /// </summary>
        [DataMember]
        public DistinguishedName DistinguishedName { get; set; }

        [DataMember]
        public bool IsIssuer { get; set; }

        [DataMember]
        public DistinguishedName IssuingDn { get; set; }

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

        [DataMember]
        public int LengthInYears { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ParentCaSlug { get; set; }

        public override string[] SlugProperties()
        {
            return new string[] { Guid.NewGuid().ToString() };
        }
    }
}
