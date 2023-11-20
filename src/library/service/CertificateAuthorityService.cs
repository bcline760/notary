﻿using System;
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
    public class CertificateAuthorityService : EntityService<CertificateAuthority>, ICertificateAuthorityService
    {
        public CertificateAuthorityService(
            ICertificateAuthorityRepository repo,
            ICertificateService caService,
            NotaryConfiguration config, ILog log) : base(repo, log)
        {
            CertificateService = caService;
            Configuration = config;
        }

        public async Task SetupCertificateAuthority(CertificateAuthoritySetup setup)
        {
            if (setup == null)
                throw new ArgumentNullException(nameof(setup));

            if (setup.Algorithm == Algorithm.RSA && !setup.KeyLength.HasValue)
                throw new ArgumentException("Must supply a key length if using RSA");

            CertificateAuthority parentCa = null;
            X509Certificate parentCertificate = null;
            if (!string.IsNullOrEmpty(setup.ParentCaSlug))
            {
                parentCa = await Repository.GetAsync(setup.ParentCaSlug);
                if (parentCa == null)
                    throw new ArgumentNullException(nameof(parentCa));

                var cert = await CertificateService.GetAsync(parentCa.CertificateSlug);
                if (cert == null)
                    throw new ArgumentNullException(nameof(cert));

                parentCertificate = GetX509FromBinary(cert.Data);
            }

            var now = DateTime.UtcNow;
            short keyUsageBits = (short)KeyPurposeFlags.CodeSigning;

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

            var caRequest = new CertificateRequest
            {
                Curve = parentCa != null ? parentCa.KeyCurve : setup.Curve,
                KeyAlgorithm = parentCa != null ? parentCa.KeyAlgorithm : setup.Algorithm,
                KeySize = parentCa != null ? parentCa.KeyLength : setup.KeyLength,
                KeyUsage = keyUsageBits,
                LengthInHours = 87600, // 10 years
                Name = setup.Name,
                ParentCertificateSlug = parentCa != null ? parentCa.CertificateSlug : null,
                RequestedBySlug = setup.Requestor,
                Subject = dn
            };

            var caCertificate = await CertificateService.IssueCertificateAsync(caRequest);

            try
            {
                
                var ca = new CertificateAuthority
                {
                    CertificateSlug = caCertificate.Slug,
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

        /// <summary>
        /// Convert raw binary data to a X.509 certificate object
        /// </summary>
        /// <param name="certificateData">The raw certificate binary</param>
        /// <returns>An X.509 certificate or null if it is not on disk</returns>
        private X509Certificate GetX509FromBinary(byte[] certificateData)
        {
            var parser = new X509CertificateParser();
            X509Certificate cert = parser.ReadCertificate(certificateData);

            return cert;
        }

        protected ICertificateService CertificateService { get; }

        protected NotaryConfiguration Configuration { get; }
    }
}
