using System;
using System.Collections.Generic;
using System.Text;

using Autofac;

namespace Notary.Service
{
    public static class RegisterModules
    {
        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new Data.IocRegistration());
            builder.RegisterModule(new IocRegistrations());
        }
    }
}
