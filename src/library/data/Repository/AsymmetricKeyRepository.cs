using System;
using Notary.Contract;
using Notary.Model;
using Notary.Data.Repository;
using MongoDB.Driver;
using Notary.Interface.Repository;
using System.Threading.Tasks;

namespace Notary.Data;

public class AsymmetricKeyRepository : BaseRepository<AsymmetricKey, AsymmetricKeyModel>, IAsymmetricKeyRepository
{
    public AsymmetricKeyRepository(IMongoDatabase db) : base(db)
    {
        var nameIndex = new CreateIndexModel<AsymmetricKeyModel>(IndexKeys.Ascending(x => x.Name));
        var notAfterIndex = new CreateIndexModel<AsymmetricKeyModel>(IndexKeys.Ascending(x => x.NotAfter));
        var notBeforeIndex = new CreateIndexModel<AsymmetricKeyModel>(IndexKeys.Ascending(x => x.NotBefore));

        Task.Run(async () => await Collection.Indexes.CreateManyAsync(new[] { nameIndex, notAfterIndex, notBeforeIndex }));
    }
}
