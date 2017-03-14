using Ninject.Activation;
using System;
using System.Reflection;
using Xamarin.Forms;

namespace Prism.Ninject.Extensions
{
    /// <summary>
    /// Creates instances of services using the
    /// <see cref="Xamarin.Forms.DependencyService"/>
    /// </summary>
    public class DependencyServiceProvider : IProvider
    {
        /// <summary>
        /// Constructs a <see cref="DependencyServiceProvider"/>
        /// </summary>
        /// <param name="type">The type that this provider resolves</param>
        public DependencyServiceProvider(Type type)
        {
            Type = type;
        }

        /// <inheritDoc />
        public Type Type { get; private set; }

        /// <inheritDoc />
        public object Create(IContext context)
        {
            return ResolveFromDependencyService(Type);
        }

        private static object ResolveFromDependencyService(Type targetType)
        {
            if (targetType.GetTypeInfo().IsInterface)
            {
                MethodInfo method = typeof(DependencyService).GetTypeInfo().GetDeclaredMethod("Get");
                MethodInfo genericMethod = method.MakeGenericMethod(targetType);
                return genericMethod.Invoke(null, new object[] { DependencyFetchTarget.GlobalInstance });
            }
            return null;
        }
    }
}
