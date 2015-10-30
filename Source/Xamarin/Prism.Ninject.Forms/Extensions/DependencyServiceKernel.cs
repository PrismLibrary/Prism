using Ninject;
using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Modules;
using System.Reflection;
using Xamarin.Forms;

namespace Prism.Ninject.Extensions
{
    /// <summary>
    /// A Ninject kernel that also resolves objects from the Xamarin.Forms
    /// <see cref="Xamarin.Forms.DependencyService"/>
    /// </summary>
    public class DependencyServiceKernel : StandardKernel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public DependencyServiceKernel(params INinjectModule[] modules) : base(modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public DependencyServiceKernel(INinjectSettings settings, params INinjectModule[] modules) : base(settings, modules)
        {
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public override IEnumerable<object> Resolve(IRequest request)
        {
            if(base.CanResolve(request))
                return base.Resolve(request);
            var result = ResolveFromDependencyService(request.Service);
            if (result == null)
                throw new ActivationException(string.Format("Could not resolve {0} in Xamarin.Forms.DependencyService", request.Service.FullName));
            return new[] { result };
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public override bool CanResolve(IRequest request)
        {
            return base.CanResolve(request) || CanResolveFromDependencyService(request.Service);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <c>true</c> implicit bindings are ignored.</param>
        /// <returns>
        /// <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            return base.CanResolve(request, ignoreImplicitBindings) || CanResolveFromDependencyService(request.Service);
        }

        private static bool CanResolveFromDependencyService(Type targetType)
        {
            return ResolveFromDependencyService(targetType) != null;
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
