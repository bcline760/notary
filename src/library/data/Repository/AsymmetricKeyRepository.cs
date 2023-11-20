using System;
using Notary.Contract;
using Notary.Model;
using Notary.Data.Repository;
using MongoDB.Driver;

namespace Notary.Data;

public class AsymmetricKeyRepository : BaseRepository<AsymmetricKey, AsymmetricKeyModel>
{
    public AsymmetricKeyRepository(IMongoDatabase db) : base(db)
    {
    }
}
