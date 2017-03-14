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
        public static Factory DependencyServiceResolverRule(Request request)
        {
            return new DelegateFactory(_ =>
            {
                var method = typeof(DependencyService).GetTypeInfo().GetDeclaredMethod("Get");
                var genericMethod = method.MakeGenericMethod(request.ServiceType);
                return genericMethod.Invoke(null, new object[] { DependencyFetchTarget.GlobalInstance });
            });
        }
    }
}