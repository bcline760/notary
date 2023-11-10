using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Notary.Configuration;
using Notary.Contract;
using Notary.Interface.Repository;
using Notary.Interface.Service;
using Notary.Logging;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

namespace Notary.Service
{
    public class CertificateAuthorityService : EntityService<CertificateAuthority>, ICertificateAuthorityService
    {
        public CertificateAuthorityService(
            ICertificateAuthorityRepository repo,
            ICertificateRepository certRepo,
            IEncryptionService encService,
            NotaryConfiguration config, ILog log) : base(repo, log)
        {
            Configuration = config;
            CertificateRepository = certRepo;
            EncryptionService = encService;
        }

        public async Task SetupCertificateAuthority(CertificateAuthoritySetup setup)
        {
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));

            if (setup.Algorithm == Algorithm.RSA && !setup.KeyLength.HasValue)
                throw new ArgumentException("Must supply a key length if using RSA");

            CertificateAuthority parentCa = null;
            if (!string.IsNullOrEmpty(setup.ParentCaSlug))
                parentCa = await Repository.GetAsync(setup.ParentCaSlug);

            var now = DateTime.UtcNow;
            var randomRoot = EncryptionService.GetSecureRandom();
            var serialNum = EncryptionService.GenerateSerialNumber(randomRoot);
            var keyPair = EncryptionService.GenerateKeyPair(randomRoot, setup.Algorithm, setup.Curve, setup.KeyLength);

            DistinguishedName issuerDn = null;
            var dn = new DistinguishedName
            {
                CommonName = setup.Name,
                Country = setup.Country,
                Locale = setup.Locale,
                Organization = setup.Organization,
                OrganizationalUnit = setup.OrganizationalUnit,
                StateProvince = setup.StateProvince
            };

            if (parentCa != null)
            {
                issuerDn = new DistinguishedName
                {
                    CommonName = parentCa.DistinguishedName.CommonName,
                    Country = parentCa.DistinguishedName.Country,
                    Locale = parentCa.DistinguishedName.Locale,
                    Organization = parentCa.DistinguishedName.Organization,
                    OrganizationalUnit = parentCa.DistinguishedName.OrganizationalUnit,
                    StateProvince = parentCa.DistinguishedName.StateProvince
                };
            }

            X509Certificate caCertificate = null;
            try
            {
                caCertificate = EncryptionService.GenerateCertificate(
                    new List<SubjectAlternativeName>(),
                    randomRoot,
                    setup.Algorithm,
                    DistinguishedName.BuildDistinguishedName(dn),
                    keyPair,
                    serialNum,
                    issuerDn != null ? DistinguishedName.BuildDistinguishedName(issuerDn) : DistinguishedName.BuildDistinguishedName(dn), //Root certs self signed
                    now.AddYears(setup.LengthInYears),
                    keyPair, //Root certs, self signed
                    serialNum,
                    true,
                    new KeyPurposeID[]
                    {
                        KeyPurposeID.IdKPCodeSigning
                    });

                var thumb = EncryptionService.GetThumbprint(caCertificate);

                short keyUsageBits = (short)KeyPurposeFlags.CodeSigning;
                var ca = new CertificateAuthority
                {
                    DistinguishedName = dn,
                    IsIssuer = setup.IsIssuer,
                    IssuingDn = issuerDn,
                    IssuingSerialNumber = caCertificate.SerialNumber.ToString(),
                    IssuingThumbprint = thumb,
                    Active = true,
                    Created = DateTime.UtcNow,
                    CreatedBySlug = setup.Requestor,
                    KeyAlgorithm = setup.Algorithm,
                    KeyCurve = setup.Curve,
                    KeyLength = setup.KeyLength,
                    Name = setup.Name,
                    ParentCaSlug = setup.ParentCaSlug
                };

                await SaveAsync(ca, setup.Requestor);

                string path = $"{Configuration.RootDirectory}/{ca.Slug}";

                string keyPath = $"{path}/keys/{thumb}.key.pem";
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(keyPath));
                EncryptionService.SavePrivateKey(keyPair, keyPath, randomRoot, Configuration.ApplicationKey);

                string certPath = $"{path}/{Constants.CertificateDirectoryPath}/{thumb}.cer";
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(certPath));
                await EncryptionService.SaveCertificateAsync(caCertificate, certPath);

                var caCert = new Certificate
                {
                    Active = true,
                    CertificateAuthoritySlug = ca.Slug,
                    Created = ca.Created,
                    CreatedBySlug = ca.CreatedBySlug,
                    IsCaCertificate = true,
                    Issuer = issuerDn != null ? issuerDn : dn,
                    KeyAlgorithm = ca.KeyAlgorithm,
                    KeyCurve = ca.KeyCurve,
                    KeyLength = ca.KeyLength,
                    KeyUsage = keyUsageBits,
                    Name = setup.Name,
                    NotAfter = now.AddYears(setup.LengthInYears),
                    NotBefore = now,
                    SerialNumber = caCertificate.SerialNumber.ToString(),
                    Subject = dn,
                    SignatureAlgorithm = caCertificate.SigAlgName,
                    Thumbprint = thumb
                };

                await CertificateRepository.SaveAsync(caCert);
            }
            catch (Exception cex)
            {
                throw cex.IfNotLoggedThenLog(Logger);
            }
        }

        public async Task<List<CaCertificateList>> GetAllAuthoritiesAndCertificate()
        {
            var caCertList = new List<CaCertificateList>();

            var caList = await ((ICertificateAuthorityRepository)Repository).GetAllAsync();
            caCertList.AddRange(caList.Select(ca => new CaCertificateList
            {
                Name = ca.Name,
                ParentCaSlug = ca.ParentCaSlug,
                Slug = ca.Slug
            }));

            foreach (var ca in caCertList)
            {
                var certificates = await CertificateRepository.GetCertificatesByCaAsync(ca.Slug);

                //Strip out the CA certificates, those will be handled in another location.
                certificates = certificates.Where(c => !c.IsCaCertificate).ToList();
                ca.CertificateCollection.AddRange(certificates);
            }

            return caCertList;
        }

        public async Task<List<CaBrief>> GetCaListBrief()
        {
            var caList = await ((ICertificateAuthorityRepository)Repository).GetAllAsync();
            var caListBrief = new List<CaBrief>();

            foreach (var ca in caList)
            {
                var brief = new CaBrief
                {
                    Name = ca.Name,
                    Slug = ca.Slug,
                    CreatedOn = ca.Created.ToString()
                };

                if (!string.IsNullOrEmpty(ca.ParentCaSlug))
                {
                    var parentCa = await GetAsync(ca.ParentCaSlug);
                    brief.ParentName = parentCa.Name;
                }

                //brief.CreatedBy = createdBy.Name;
                caListBrief.Add(brief);
            }

            return caListBrief;
        }

        protected NotaryConfiguration Configuration { get; }

        protected ICertificateRevokeService CertificateRevokeService { get; }

        protected ICertificateRepository CertificateRepository { get; }

        protected IEncryptionService EncryptionService { get; }
    }
}
