using DryIoc;
using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.DryIoc.Forms.Tests.Mocks
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
            Container.Register<IServiceMock, ServiceMock>();
            Container.RegisterTypeForNavigation<ViewMock>("view");
            Container.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            Container.Register<AutowireViewModel>();
            Container.Register<ViewModelAMock>();
            Container.Register<ViewModelBMock>(serviceKey: ViewModelBMock.Key);
            Container.Register<ConstructorArgumentViewModel>();
            Container.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
            Container.RegisterTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
            Container.Register<ModuleMock>(Reuse.Singleton);
        }

        public INavigationService CreateNavigationServiceForPage(Page page)
        {
            return CreateNavigationService(page);
        }
    }
}