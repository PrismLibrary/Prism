using System;
using System.Linq;
using SimpleInjector;
using Prism.AppModel;
using Prism.Common;
using Prism.SimpleInjector.Modularity;
using Prism.SimpleInjector.Navigation;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SimpleInjector.Lifestyles;
using Xamarin.Forms;
using DependencyService = Prism.Services.DependencyService;

namespace Prism.SimpleInjector
{
    /// <summary>
    /// Application base class using SimpleInjector
    /// </summary>
    public abstract class PrismApplication : PrismApplicationBase<Container>, IDisposable
    {
        private Scope _scope;

        /// <summary>
        /// Create a new instance of <see cref="PrismApplication"/>
        /// </summary>
        /// <param name="platformInitializer">Class to initialize platform instances</param>
        /// <remarks>
        /// The method <see cref="IPlatformInitializer.RegisterTypes(IContainer)"/> will be called after <see cref="PrismApplication.RegisterTypes()"/> 
        /// to allow for registering platform specific instances.
        /// </remarks>
        protected PrismApplication(IPlatformInitializer platformInitializer = null)
            : base(platformInitializer)
        {

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Container != null)
            {
                Container.Dispose();
                Container = null;
            }

            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Create a default instance of <see cref="Container" />
        /// </summary>
        /// <returns>An instance of <see cref="Container" /></returns>
        protected override Container CreateContainer() => new Container();

        protected override IModuleManager CreateModuleManager()
        {
            return Container.GetInstance<IModuleManager>();
        }

        protected override void ConfigureContainer()
        {
            Container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            Container.Register(() => Logger, Lifestyle.Singleton);
            Container.Register(() => ModuleCatalog, Lifestyle.Singleton);
            Container.Register<INavigationService, SimpleInjectorPageNavigationService>(Lifestyle.Scoped);
            Container.Register<ISimpleInjectorPageNavigationService, SimpleInjectorPageNavigationService>(Lifestyle.Scoped);
            Container.Register<IApplicationProvider, ApplicationProvider>(Lifestyle.Singleton);
            Container.Register<IApplicationStore, ApplicationStore>(Lifestyle.Singleton);
            Container.Register<IModuleManager, ModuleManager>(Lifestyle.Singleton);
            Container.Register<IModuleInitializer, SimpleInjectorModuleInitializer>(Lifestyle.Singleton);
            Container.Register<IEventAggregator, EventAggregator>(Lifestyle.Singleton);
            Container.Register<IDependencyService, DependencyService>(Lifestyle.Singleton);
            Container.Register<IPageDialogService, PageDialogService>(Lifestyle.Singleton);
            Container.Register<IDeviceService, DeviceService>(Lifestyle.Singleton);
        }

        protected override void InitializeModules()
        {
            if (ModuleCatalog.Modules.Any())
            {
                var manager = Container.GetInstance<IModuleManager>();
                manager.Run();
            }
        }

        /// <summary>
        /// Create instance of <see cref="INavigationService"/>
        /// </summary>
        /// <remarks>
        /// The <see cref="_navigationServiceKey"/> is used as service key when resolving
        /// </remarks>
        /// <returns>Instance of <see cref="INavigationService"/></returns>
        protected override INavigationService CreateNavigationService()
        {
            if (Lifestyle.Scoped.GetCurrentScope(Container) == null)
            {
                _scope = ThreadScopedLifestyle.BeginScope(Container);
            }

            return Container.GetInstance<ISimpleInjectorPageNavigationService>();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) =>
            {
                using (ThreadScopedLifestyle.BeginScope(Container))
                {
                    var page = view as Page;

                    if (page != null)
                    {
                        CreateNavigationService(page);
                    }

                    return Container.GetInstance(type);
                }
            });
        }
    }
}