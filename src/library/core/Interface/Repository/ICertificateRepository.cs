using Notary.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Interface.Repository
{
    public interface ICertificateRepository : IRepository<Certificate>
    {
        /// <summary>
        /// Gets the signing certificate used to sign certificates
        /// </summary>
        /// <returns>The signing certificate or null if it does not exist.</returns>
        Task<Certificate> GetSigningCertificateAsync();

        /// <summary>
        /// Get all the certificates that are a part of the CA.
        /// </summary>
        /// <param name="slug">The slug of the CA</param>
        /// <returns>List of certificates in the CA</returns>
        Task<List<Certificate>> GetCertificatesByCaAsync(string caSlug);
    }
}
