using MongoDB.Driver;

using Notary.Contract;
using Notary.Interface.Repository;
using Notary.Model;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Data.Repository
{
    internal class CertificateAuthorityRepository : BaseRepository<CertificateAuthority, CertificateAuthorityModel>, ICertificateAuthorityRepository
    {
        public CertificateAuthorityRepository(IMongoDatabase db) : base(db)
        {
        }
    }
}
