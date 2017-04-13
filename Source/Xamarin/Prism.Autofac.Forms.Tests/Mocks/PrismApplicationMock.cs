using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac.Forms.Tests.Mocks
{
    public class PrismApplicationMock : PrismApplication
    {
        public PrismApplicationMock()
        { }

        public PrismApplicationMock(Page startPage)
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
            FormsDependencyService.Register<IDependencyServiceMock>(new DependencyServiceMock());

            Container.RegisterType<ServiceMock>().As<IServiceMock>();
            Container.RegisterType<AutowireViewModel>();
            Container.RegisterType<ViewModelAMock>();
            Container.Register(ctx => new ViewModelBMock()).Named<ViewModelBMock>(ViewModelBMock.Key);
            Container.RegisterType<ConstructorArgumentViewModel>();
            Container.RegisterType<ModuleMock>().SingleInstance();
            Container.Register(ctx => FormsDependencyService.Get<IDependencyServiceMock>())
                .As<IDependencyServiceMock>();

            Container.RegisterTypeForNavigation<ViewMock>("view");
            Container.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            Container.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
            Container.RegisterTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
        }

        public INavigationService CreateNavigationServiceForPage()
        {
            return CreateNavigationService();
        }
    }
}
