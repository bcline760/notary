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
                var connectionString = config.Database.ConnectionString;

                var settings = MongoClientSettings.FromUrl(MongoUrl.Create(connectionString));

                if (!string.IsNullOrEmpty(config.Database.DatabaseName) && !string.IsNullOrEmpty(config.Database.Username))
                {
                    var credential = MongoCredential.CreateCredential("admin", config.Database.Username, config.Database.Password);
                    settings.Credential = credential;
                }

                IMongoClient client = new MongoClient(settings);
                IMongoDatabase db = client.GetDatabase(config.Database.DatabaseName);
                return db;
            }).As<IMongoDatabase>().SingleInstance();

            builder.RegisterType<CertificateAuthorityRepository>().As<ICertificateAuthorityRepository>().InstancePerLifetimeScope();
            builder.RegisterType<CertificateRepository>().As<ICertificateRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RevocatedCertificateRepository>().As<IRevocatedCertificateRepository>().InstancePerLifetimeScope();
        }
    }
}

