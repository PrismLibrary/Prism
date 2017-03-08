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
            var builder = new ContainerBuilder();

            builder.RegisterType<ServiceMock>().As<IServiceMock>();
            builder.RegisterType<AutowireViewModel>();
            builder.RegisterType<ViewModelAMock>();
            builder.Register(ctx => new ViewModelBMock()).Named<ViewModelBMock>(ViewModelBMock.Key);
            builder.RegisterType<ConstructorArgumentViewModel>();
            builder.RegisterType<ModuleMock>().SingleInstance();

            builder.Update(Container);

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