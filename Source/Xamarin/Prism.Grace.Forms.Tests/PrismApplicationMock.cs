using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Grace.Forms.Tests.Mocks
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
            containerRegistry.Register<IServiceMock, ServiceMock>();
            containerRegistry.RegisterForNavigation<ViewMock>("view");
            containerRegistry.RegisterForNavigation<ViewAMock, ViewModelAMock>();
            containerRegistry.Register<AutowireViewModel>();
            containerRegistry.Register<ViewModelAMock>();
            containerRegistry.Register<ViewModelBMock>(ViewModelBMock.Key);
            containerRegistry.Register<ConstructorArgumentViewModel>();
            containerRegistry.RegisterForNavigation<AutowireView, AutowireViewModel>();
            containerRegistry.RegisterForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
            containerRegistry.RegisterSingleton<ModuleMock>();

            DependencyService.Register<IDependencyServiceMock, DependencyServiceMock>();
        }

        public INavigationService CreateNavigationServiceForPage(Page page)
        {
            return ((IContainerExtension)Container).CreateNavigationService(page);
        }
    }
}
