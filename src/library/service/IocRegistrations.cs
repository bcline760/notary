using Autofac;
using Autofac.Extras.DynamicProxy;

using Notary.Interface.Service;
using Notary.IOC;

namespace Notary.Service
{
    public class IocRegistrations : Module, IRegister
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AsymmetricKeyService>()
                .As<IAsymmetricKeyService>()
                .EnableInterfaceInterceptors()
                .InstancePerLifetimeScope();

            builder.RegisterType<CertificateAuthorityService>()
                .As<ICertificateAuthorityService>()
                .EnableInterfaceInterceptors()
                .InstancePerLifetimeScope();

            builder.RegisterType<CertificateRevokeService>()
                .As<ICertificateRevokeService>()
                .EnableInterfaceInterceptors()
                .InstancePerLifetimeScope();

            builder.RegisterType<CertificateService>()
                .As<ICertificateService>()
                .EnableInterfaceInterceptors()
                .InstancePerLifetimeScope();
        }
    }
}
