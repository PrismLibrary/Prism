using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Components;

using Binding = Ninject.Planning.Bindings.Binding;

namespace Prism.Ninject.Extensions
{
    /// <summary>
    /// Resolves missing dependencies using <see cref="Xamarin.Forms.DependencyService"/>
    /// </summary>
    public class DependencyServiceBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        /// <inheritDoc />
        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
        {
            var service = request.Service;
            if (!service.GetTypeInfo().IsInterface)
            {
                return Enumerable.Empty<IBinding>();
            }

            return new[] { new Binding(service)
            {
                ProviderCallback = CreateDependencyServiceProvider(service)
            }};
        }

        /// <inheritDoc />
        private static Func<IContext, IProvider> CreateDependencyServiceProvider(Type targetType)
        {
            var provider = new DependencyServiceProvider(targetType);
            return ctx => provider;
        }
    }
}
