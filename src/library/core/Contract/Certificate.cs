using System;
using System.Collections.Generic;
using System.Linq;

using Notary.Model;
using Newtonsoft.Json;
using System.Text;

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
            Data = model.Data;
            IssuingSlug = model.IssuingSlug;
            IsCaCertificate = model.IsCaCertificate;
            KeySlug = model.KeySlug;
            KeyUsages = model.KeyUsages;
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

        /// <summary>
        /// Get or set the certificate binary
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public string Data { get; set; }

        /// <summary>
        /// Get or set whether this certificate is a CA
        /// </summary>
        [JsonProperty("is_ca", Required = Required.Always)]
        public bool IsCaCertificate { get; set; }

        /// <summary>
        /// Denotes the X509 Certificate common name
        /// </summary>
        [JsonProperty("issuer", Required = Required.Always)]
        public DistinguishedName Issuer
        {
            get; set;
        }

        [JsonProperty("iss_slug", Required = Required.Always)]
        public string IssuingSlug { get; set; }

        [JsonProperty("key_slug", Required = Required.Always)]
        public string KeySlug { get; set; }

        [JsonProperty("key_usage", Required = Required.Always)]
        public List<string> KeyUsages
        {
            get; set;
        }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// The certificate is not valid before this given date.
        /// </summary>
        [JsonProperty("not_before", Required = Required.Always)]
        public DateTime NotBefore
        {
            get; set;
        }

        /// <summary>
        /// The certificate is not valid after the given date
        /// </summary>
        [JsonProperty("note_after", Required = Required.Always)]
        public DateTime NotAfter
        {
            get; set;
        }

        /// <summary>
        /// Get or set the date the certificate was issued
        /// </summary>
        [JsonProperty("rev_date", Required = Required.Always)]
        public DateTime? RevocationDate
        {
            get; set;
        }

        [JsonProperty("sn", Required = Required.Always)]
        public string SerialNumber
        {
            get; set;
        }

        [JsonProperty("sig_alg", Required = Required.Always)]
        public string SignatureAlgorithm { get; set; }

        [JsonProperty("sub", Required = Required.Always)]
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
            return new string[] { Guid.NewGuid().ToString("N") };
        }
    }
}