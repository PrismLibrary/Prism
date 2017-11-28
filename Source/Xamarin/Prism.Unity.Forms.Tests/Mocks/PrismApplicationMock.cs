using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;
using Prism.Ioc;

namespace Prism.Unity.Forms.Tests.Mocks
{
    public class PrismApplicationMock : PrismApplication
    {
        public PrismApplicationMock()
        {
        }

        public PrismApplicationMock(Page startPage) : this()
        {
            Current.MainPage = startPage;
        }

        public new INavigationService NavigationService => base.NavigationService;

        public bool Initialized { get; private set; }

        protected override void OnInitialized()
        {
            Initialized = true;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(new ModuleInfo(typeof(ModuleMock))
            {
                InitializationMode = InitializationMode.WhenAvailable,
                ModuleName = "ModuleMock"
            });
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterType<IServiceMock, ServiceMock>();
            containerRegistry.RegisterTypeForNavigation<ViewMock>("view");
            containerRegistry.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            containerRegistry.RegisterType<AutowireViewModel>();
            containerRegistry.RegisterType<ViewModelAMock>();
            containerRegistry.RegisterType<ViewModelBMock>(ViewModelBMock.Key);
            containerRegistry.RegisterType<ConstructorArgumentViewModel>();
            containerRegistry.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
            containerRegistry.RegisterTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
            containerRegistry.RegisterSingleton<ModuleMock>();

            DependencyService.Register<IDependencyServiceMock, DependencyServiceMock>();
        }

        public INavigationService CreateNavigationServiceForPage(Page page)
        {
            return ((IContainerExtension)Container).CreateNavigationService(page);
        }
    }
}
