using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using log4net;
using Notary.Configuration;
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
                .InstancePerMatchingLifetimeScope();

            builder.RegisterType<CertificateAuthorityService>()
                .As<ICertificateAuthorityService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CertificateRevokeService>()
                .As<ICertificateRevokeService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CertificateService>()
                .As<ICertificateService>()
                .InstancePerLifetimeScope();
        }
    }
}
