using System;
using System.Collections.Generic;
using System.Linq;

using Notary.Model;
using Newtonsoft.Json;

namespace Notary.Contract
{
    /// <summary>
    /// A basic certificate object
    /// </summary>
    public class Certificate : Entity
    {
        public Certificate()
        {

        }

        public Certificate(CertificateModel model) : base(model)
        {
            CertificateAuthoritySlug = model.CertificateAuthoritySlug;
            IsCaCertificate = model.IsCaCertificate;
            KeyAlgorithm = model.Algorithm;
            KeyCurve = model.EllipticCurve;
            Name = model.Name;
            NotAfter = model.NotAfter;
            NotBefore = model.NotBefore;
            RevocationDate = model.RevocationDate;
            SerialNumber = model.SerialNumber;
            SignatureAlgorithm = model.SignatureAlgorithm;
            RevocationDate = model.RevocationDate;
            Thumbprint = model.Thumbprint;

            Issuer = new DistinguishedName
            {
                CommonName = model.Issuer.CommonName,
                Country = model.Issuer.Country,
                Locale = model.Issuer.Locale,
                Organization = model.Issuer.Organization,
                OrganizationalUnit = model.Issuer.OrganizationalUnit,
                StateProvince = model.Issuer.StateProvince
            };

            Subject = new DistinguishedName
            {
                CommonName = model.Subject.CommonName,
                Country = model.Subject.Country,
                Locale = model.Subject.Locale,
                Organization = model.Subject.OrganizationalUnit,
                OrganizationalUnit = model.Subject.OrganizationalUnit,
                StateProvince = model.Subject.StateProvince
            };

            if (model.SubjectAlternativeNames != null)
            {
                SubjectAlternativeNames = new List<SubjectAlternativeName>();
                SubjectAlternativeNames.AddRange(model.SubjectAlternativeNames.Select(s => new SubjectAlternativeName
                {
                    Name = s.Name,
                    Kind = s.Kind
                }));
            }
        }

        [JsonProperty("caSlug", Required = Required.Always)]
        public string CertificateAuthoritySlug { get; set; }

        [JsonProperty("isCa", Required = Required.Always)]
        public bool IsCaCertificate { get; set; }

        /// <summary>
        /// Denotes the X509 Certificate common name
        /// </summary>
        [JsonProperty("issuer", Required = Required.Always)]
        public DistinguishedName Issuer
        {
            get; set;
        }

        [JsonProperty("alg", Required = Required.Always)]
        public Algorithm KeyAlgorithm
        {
            get; set;
        }

        /// <summary>
        /// The elliptic curve to use if EC is used to generate the keys
        /// </summary>
        [JsonProperty("curve", Required = Required.AllowNull)]
        public EllipticCurve? KeyCurve { get; set; }

        /// <summary>
        /// The length of the RSA key if RSA is used to generate the keys
        /// </summary>
        public int? KeyLength { get; set; }

        [JsonProperty("keyUsage", Required = Required.Always)]
        public short KeyUsage
        {
            get; set;
        }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// The certificate is not valid before this given date.
        /// </summary>
        [JsonProperty("notBefore", Required = Required.Always)]
        public DateTime NotBefore
        {
            get; set;
        }

        /// <summary>
        /// The certificate is not valid after the given date
        /// </summary>
        [JsonProperty("notAfter", Required = Required.Always)]
        public DateTime NotAfter
        {
            get; set;
        }

        /// <summary>
        /// Get or set the date the certificate was issued
        /// </summary>
        [JsonProperty("revocationDate", Required = Required.Always)]
        public DateTime? RevocationDate
        {
            get; set;
        }

        [JsonProperty("serialNumber", Required = Required.Always)]
        public string SerialNumber
        {
            get; set;
        }

        [JsonProperty("sigAlg", Required = Required.Always)]
        public string SignatureAlgorithm { get; set; }

        [JsonProperty("subject", Required = Required.Always)]
        public DistinguishedName Subject { get; set; }

        [JsonProperty("san", Required = Required.AllowNull)]
        public List<SubjectAlternativeName> SubjectAlternativeNames
        {
            get; set;
        }

        [JsonProperty("thumbprint", Required = Required.Always)]
        public string Thumbprint
        {
            get; set;
        }

        public override string[] SlugProperties()
        {
            return new string[] { Guid.NewGuid().ToString() };
        }
    }
}