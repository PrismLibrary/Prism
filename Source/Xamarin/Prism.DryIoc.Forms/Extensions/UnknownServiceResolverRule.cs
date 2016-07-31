using System;
using System.Reflection;
using DryIoc;
using Xamarin.Forms;

namespace Prism.DryIoc.Extensions
{
    /// <summary>
    /// Allow for resolving missing types using <see cref="DependencyService"/>
    /// </summary>
    public static class UnknownServiceResolverRule
    {
        public static Rules DependencyServiceResolverRule;

        static UnknownServiceResolverRule()
        {
            DependencyServiceResolverRule = Rules.Default.WithUnknownServiceResolvers(request => new DelegateFactory(_ =>
            {
                var targetType = request.ServiceType;
                if (!targetType.GetTypeInfo().IsInterface)
                {
                    return null;
                }
                var method = typeof(DependencyService).GetTypeInfo().GetDeclaredMethod("Get");
                var genericMethod = method.MakeGenericMethod(targetType);
                return genericMethod.Invoke(null, new object[] { DependencyFetchTarget.GlobalInstance });
            }));
        }
    }
}