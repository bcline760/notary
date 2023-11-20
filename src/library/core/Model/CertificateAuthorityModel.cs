﻿using MongoDB.Bson.Serialization.Attributes;

using Notary.Contract;

using System.Text;

namespace Notary.Model
{
    [Collection("certificate_authorities")]
    public class CertificateAuthorityModel : BaseModel
    {
        public CertificateAuthorityModel()
        {

        }

        public CertificateAuthorityModel(CertificateAuthority ca) : base(ca)
        {
            DistinguishedName = new DistinguishedNameModel(ca.DistinguishedName);
            if (ca.IssuingDn != null)
                IssuingDn = new DistinguishedNameModel(ca.IssuingDn);
            IssuingSerialNumber = ca.IssuingSerialNumber;
            IssuingThumbprint = ca.IssuingThumbprint;
            IsIssuer = ca.IsIssuer;
            KeyAlgorithm = ca.KeyAlgorithm;
            KeyCurve = ca.KeyCurve;
            KeyLength = ca.KeyLength;
            Name = ca.Name;
            ParentCaSlug = ca.ParentCaSlug;
        }

        [BsonElement("dn")]
        public DistinguishedNameModel DistinguishedName { get; set; }

        [BsonElement("is_issuer")]
        public bool IsIssuer { get; set; }

        [BsonElement("issuing_dn")]
        public DistinguishedNameModel IssuingDn { get; set; }

        [BsonElement("issuing_sn")]
        public string IssuingSerialNumber { get; set; }

        [BsonElement("issuing_thumbprint")]
        public string IssuingThumbprint { get; set; }

        [BsonElement("key_alg")]
        public Algorithm KeyAlgorithm
        {
            get; set;
        }

        /// <summary>
        /// The elliptic curve to use if EC is used to generate the keys
        /// </summary>
        [BsonElement("curve")]
        public EllipticCurve? KeyCurve { get; set; }

        /// <summary>
        /// The length of the RSA key if RSA is used to generate the keys
        /// </summary>
        [BsonElement("key_len")]
        public int? KeyLength { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("parent_slug")]
        public string ParentCaSlug { get; set; }
    }
}
