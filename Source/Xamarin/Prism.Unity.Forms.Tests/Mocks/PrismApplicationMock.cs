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
            moduleCatalog.AddModule(new ModuleInfo
            {
                InitializationMode = InitializationMode.WhenAvailable,
                ModuleName = "ModuleMock",
                ModuleType = typeof(ModuleMock)
            });
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IServiceMock, ServiceMock>();
            Container.RegisterTypeForNavigation<ViewMock>("view");
            Container.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            Container.RegisterType<AutowireViewModel>();
            Container.RegisterType<ViewModelAMock>();
            Container.RegisterType<ViewModelBMock>(ViewModelBMock.Key);
            Container.RegisterType<ConstructorArgumentViewModel>();
            Container.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
            Container.RegisterTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
            Container.RegisterSingleton<ModuleMock>();

            DependencyService.Register<IDependencyServiceMock, DependencyServiceMock>();
        }

        public INavigationService CreateNavigationServiceForPage(Page page)
        {
            return CreateNavigationService(page);
        }
    }
}
