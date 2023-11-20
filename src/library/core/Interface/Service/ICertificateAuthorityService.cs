using Notary.Contract;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Interface.Service
{
    public interface ICertificateAuthorityService : IEntityService<CertificateAuthority>
    {
        /// <summary>
        /// Get a brief list of certificate authorities
        /// </summary>
        /// <returns>A brief list of certificate authorities</returns>
        Task<List<CaBrief>> GetCaListBrief();

        /// <summary>
        /// Generate the CA certificates as needed for issuing certificates
        /// </summary>
        /// <param name="root"></param>
        /// <param name="intermediate"></param>
        Task SetupCertificateAuthority(CertificateAuthoritySetup setup);
    }
}
