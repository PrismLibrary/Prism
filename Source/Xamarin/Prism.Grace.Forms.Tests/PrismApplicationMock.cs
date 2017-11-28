using Grace.DependencyInjection;
using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
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

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog.AddModule(new ModuleInfo
            {
                InitializationMode = InitializationMode.WhenAvailable,
                ModuleName = "ModuleMock",
                ModuleType = typeof(ModuleMock)
            });
        }

        protected override void RegisterTypes()
        {
            Container.Configure(c => c.ExportAs<ServiceMock, IServiceMock>());
            Container.ConfigureTypeForNavigation<ViewMock>("view");
            Container.ConfigureTypeForNavigation<ViewAMock, ViewModelAMock>();
            Container.Configure(c => c.Export<AutowireViewModel>());
            Container.Configure(c => c.Export<ViewModelAMock>());
            Container.Configure(c => c.Export<ViewModelBMock>().AsKeyed(typeof(ViewModelBMock), ViewModelBMock.Key));
            Container.Configure(c => c.Export<ConstructorArgumentViewModel>());
            Container.ConfigureTypeForNavigation<AutowireView, AutowireViewModel>();
            Container.ConfigureTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
            Container.Configure(c => c.Export<ModuleMock>().Lifestyle.Singleton());

            DependencyService.Register<IDependencyServiceMock, DependencyServiceMock>();
        }

        public INavigationService CreateNavigationServiceForPage(Page page)
        {
            return CreateNavigationService(page);
        }
    }
}
