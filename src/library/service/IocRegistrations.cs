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

            // TODO: Figure out how to use both System and LDAP
            builder.RegisterType<SystemLoginService>()
                .As<ILoginService>()
                .EnableClassInterceptors()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserService>()
                .As<IUserService>()
                .EnableClassInterceptors()
                .InstancePerLifetimeScope();
        }
    }
}
