using Autofac;

using Notary.IOC;
using Notary.IOC.Interceptor;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary
{
    public class IocRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(a => new NotaryAuthorization());
        }
    }
}
 