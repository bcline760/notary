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

namespace Notary.Service
{
    public class CertificateAuthorityService : EntityService<CertificateAuthority>, ICertificateAuthorityService
    {
        public CertificateAuthorityService(
            ICertificateAuthorityRepository repo,
            NotaryConfiguration config, ILog log) : base(repo, log)
        {
            Configuration = config;
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

            try
            {
                short keyUsageBits = (short)KeyPurposeFlags.CodeSigning;
                var ca = new CertificateAuthority
                {
                    DistinguishedName = dn,
                    IsIssuer = setup.IsIssuer,
                    IssuingDn = issuerDn,
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
            }
            catch (Exception cex)
            {
                throw cex.IfNotLoggedThenLog(Logger);
            }
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
    }
}
