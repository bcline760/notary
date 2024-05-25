using System;
using Castle.DynamicProxy;
using Castle.Core.Internal;

namespace Notary.IOC.Interceptor
{
    public class NotaryAuthorization : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var attribute = invocation.Method.GetAttribute<NotaryAuthorizationAttribute>();
            if (attribute != null)
            {
                if (attribute.AllowAnonymous)
                {
                    invocation.Proceed();
                    return;
                }
            }
            invocation.Proceed();
        }
    }
}
