using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using Prism.Common;
using Prism.Ioc;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Prism.DryIoc
{
    /// <summary>
    /// Application base class using DryIoc
    /// </summary>
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

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/> for DryIoc
        /// </summary>
        /// <returns></returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

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

                foreach(var dependency in dependencies)
                {
                    if(dependency is Android.Content.Context context)
                    {
                        var resolver = container.Resolve<Func<Android.Content.Context, object>>(type);
                        return resolver.Invoke(context);
                    }
                }
                container.Resolve<ILoggerFacade>().Log($"Could not locate an Android.Content.Context to resolve {type.Name}", Category.Warn, Priority.High);
                return container.Resolve(type);
            });
        }
#endif

        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <returns>An instance of <see cref="Rules" /></returns>
        protected virtual Rules CreateContainerRules() => Rules.Default.WithAutoConcreteTypeResolution()
                                                                       .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                                       .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);

        /// <summary>
        /// Configures the Container.
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.GetContainer().Register<INavigationService, PageNavigationService>();
            containerRegistry.GetContainer().Register<INavigationService>(
                made: Made.Of(() => SetPage(Arg.Of<INavigationService>(), Arg.Of<Page>())),
                setup: Setup.Decorator);
        }

        internal static INavigationService SetPage(INavigationService navigationService, Page page)
        {
            if (navigationService is IPageAware pageAware)
            {
                pageAware.Page = page;
            }

            return navigationService;
        }
    }
}