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

using Org.BouncyCastle.X509;

namespace Notary.Service
{
    public class CertificateAuthorityService : CryptographicEntityService<CertificateAuthority>, ICertificateAuthorityService
    {
        public CertificateAuthorityService(
            ICertificateAuthorityRepository repo,
            ICertificateService caService,
            NotaryConfiguration config, ILog log) : base(repo, log)
        {
            CertificateService = caService;
            Configuration = config;
        }

        public override async Task SaveAsync(CertificateAuthority entity, string updatedBySlug)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.KeyAlgorithm == Algorithm.RSA && !entity.KeyLength.HasValue)
                throw new ArgumentException("Must supply a key length if using RSA");

            CertificateAuthority parentCa = null;
            if (!string.IsNullOrEmpty(entity.ParentCaSlug))
            {
                parentCa = await Repository.GetAsync(entity.ParentCaSlug);
                if (parentCa == null)
                    throw new ArgumentNullException(nameof(parentCa));

                entity.KeyCurve = parentCa.KeyCurve;
                entity.KeyAlgorithm = parentCa.KeyAlgorithm;
                entity.KeyLength = parentCa.KeyLength;
                entity.ParentCaSlug = parentCa.Slug;
            }

            var now = DateTime.UtcNow;

            var caRequest = new CertificateRequest
            {
                Curve = entity.KeyCurve,
                IsCaCertificate = true,
                KeyAlgorithm = entity.KeyAlgorithm,
                KeySize = entity.KeyLength,
                KeyUsages = new List<string>
                {
                    "1.3.6.1.5.5.7.3.3" //Code signing
                },
                LengthInHours = 8760 * entity.LengthInYears,
                Name = entity.Name,
                ParentCertificateSlug = parentCa != null ? parentCa.CertificateSlug : null,
                RequestedBySlug = entity.CreatedBySlug,
                Subject = entity.DistinguishedName
            };

            var caCertificate = await CertificateService.IssueCertificateAsync(caRequest);
            entity.IssuingDn = parentCa != null ? parentCa.DistinguishedName : null;
            entity.CertificateSlug = caCertificate.Slug;

            await base.SaveAsync(entity, updatedBySlug);
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

        protected ICertificateService CertificateService { get; }

        protected NotaryConfiguration Configuration { get; }
    }
}
