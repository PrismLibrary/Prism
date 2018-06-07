using Prism.Ioc;
using Unity;
#if __ANDROID__
using System;
using Prism.Logging;
using Unity.Resolution;
using Xamarin.Forms.Internals;
#endif

namespace Prism.Unity
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Initializes a new instance of PrismApplication using the default constructor
        /// </summary>
        protected PrismApplication() 
            : base() { }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplication" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        protected PrismApplication(IPlatformInitializer platformInitializer)
            : base(platformInitializer) { }

        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplication" /> with a <see cref="IPlatformInitializer" />.
        /// Used when there are specific types that need to be registered on the platform.
        /// Also determines whether to set the <see cref="DependencyResolver" /> for resolving Renderers and Platform Effects.
        /// </summary>
        /// <param name="platformInitializer">The <see cref="IPlatformInitializer"/>.</param>
        /// <param name="setFormsDependencyResolver">Should <see cref="PrismApplication" /> set the <see cref="DependencyResolver" />.</param>
        protected PrismApplication(IPlatformInitializer platformInitializer, bool setFormsDependencyResolver)
            : base(platformInitializer, setFormsDependencyResolver) { }

#if __ANDROID__
        /// <summary>
        /// Sets the <see cref="DependencyResolver" /> to use the App Container for resolving types
        /// </summary>
        protected override void SetDependencyResolver(IContainerProvider containerProvider)
        {
            base.SetDependencyResolver(containerProvider);
            DependencyResolver.ResolveUsing((Type type, object[] dependencies) =>
            {
                var container = containerProvider.GetContainer();
                ParameterOverrides overrides = null;

                foreach(var dependency in dependencies)
                {
                    if(dependency is Android.Content.Context context)
                    {
                        if (overrides != null)
                            container.Resolve<ILoggerFacade>().Log($"An Android.Content.Context has already been provided to resolve {type.Name}", Category.Warn, Priority.High);
                        overrides = new ParameterOverrides()
                        {
                            { "context", context }
                        };
                    }
                    else
                    {
                        container.Resolve<ILoggerFacade>().Log($"Resolving an unknown type {dependency.GetType().Name}", Category.Warn, Priority.High);
                    }
                }

                return container.Resolve(type, overrides);
            });
        }
#endif

        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension(new UnityContainer());
        }
    }
}
