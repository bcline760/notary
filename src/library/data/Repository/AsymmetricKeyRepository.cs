using System;
using Notary.Contract;
using Notary.Model;
using Notary.Data.Repository;
using MongoDB.Driver;
using Notary.Interface.Repository;

namespace Notary.Data;

public class AsymmetricKeyRepository : BaseRepository<AsymmetricKey, AsymmetricKeyModel>, IAsymmetricKeyRepository
{
    public AsymmetricKeyRepository(IMongoDatabase db) : base(db)
    {
    }
}
