using Autofac;
using Prism.Ioc;
#if __ANDROID__
using Autofac.Core;
using Prism.Logging;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;
#endif

namespace Prism.Autofac
{
    /// <summary>
    /// Application base class using Autofac
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrismApplication" /> using the default constructor
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
                var parameters = new List<Parameter>();
                foreach(var dependency in dependencies)
                {
                    if(dependency is Android.Content.Context context)
                    {
                        parameters.Add(new TypedParameter(typeof(Android.Content.Context), context));
                    }
                    else
                    {
                        container.Resolve<ILoggerFacade>().Log($"Resolving an unknown type {dependency.GetType().Name}", Category.Warn, Priority.High);
                        parameters.Add(new TypedParameter(dependency.GetType(), dependency));
                    }
                }

                return container.Resolve(type, parameters.ToArray());
            });
        }
#endif

    /// <summary>
    /// Creates the <see cref="IAutofacContainerExtension"/>
    /// </summary>
    /// <returns></returns>
    protected override IContainerExtension CreateContainerExtension()
        {
            return new AutofacContainerExtension(new ContainerBuilder());
        }
    }
}
