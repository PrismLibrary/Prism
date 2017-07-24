using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;
using Autofac;

namespace Prism.Autofac.Forms.Tests.Mocks
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
            Builder.RegisterType<ServiceMock>().As<IServiceMock>();
            Builder.RegisterType<AutowireViewModel>();
            Builder.RegisterType<ViewModelAMock>();
            Builder.RegisterType<ViewModelBMock>().Named<ViewModelBMock>(ViewModelBMock.Key);
            Builder.RegisterType<ConstructorArgumentViewModel>();
            Builder.RegisterType<ModuleMock>().SingleInstance();

            Builder.RegisterTypeForNavigation<ViewMock>("view");
            Builder.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            Builder.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
            Builder.RegisterTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();

            DependencyService.Register<IDependencyServiceMock, DependencyServiceMock>();
        }

        public INavigationService CreateNavigationServiceForPage()
        {
            return CreateNavigationService();
        }
    }
}