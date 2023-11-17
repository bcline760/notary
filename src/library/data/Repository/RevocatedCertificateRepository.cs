using System;
using Notary.Contract;
using Notary.Model;
using Notary.Interface.Repository;

using MongoDB.Driver;
using AutoMapper;

namespace Notary.Data.Repository
{
    public class RevocatedCertificateRepository : BaseRepository<RevocatedCertificate, RevocatedCertificateModel>, IRevocatedCertificateRepository
    {
        public RevocatedCertificateRepository(IMongoDatabase db):base(db)
        {
        }
    }
}
