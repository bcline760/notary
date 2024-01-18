using Autofac;

using MongoDB.Driver;
using Notary.Configuration;

using Notary.Data.Repository;
using Notary.Interface.Repository;
using Notary.IOC;

namespace Notary.Data
{
    public class IocRegistration : Module, IRegister
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(r =>
            {
                var config = r.Resolve<NotaryConfiguration>();
                var settings = MongoClientSettings.FromConnectionString(config.Database.ConnectionString);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                IMongoClient client = new MongoClient(settings);
                IMongoDatabase db = client.GetDatabase(config.Database.DatabaseName);
                return db;
            }).As<IMongoDatabase>().SingleInstance();

            builder.RegisterType<AsymmetricKeyRepository>().As<IAsymmetricKeyRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CertificateAuthorityRepository>().As<ICertificateAuthorityRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CertificateRepository>().As<ICertificateRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RevocatedCertificateRepository>().As<IRevocatedCertificateRepository>().InstancePerLifetimeScope();
        }
    }
}

