using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;
using Prism.Logging;
using Prism.Forms.Tests.Mocks.Logging;
using Prism.Mvvm;
#if Autofac
using Prism.Autofac;
using Autofac;
#elif DryIoc
using Prism.DryIoc;
using DryIoc;
#elif Ninject
using Prism.Ninject;
using Ninject;
#elif Unity
using Prism.Unity;
using Unity;
#endif

namespace Prism.DI.Forms.Tests
{
    public class PrismApplicationMockPlatformAware : PrismApplicationMock
    {
        public PrismApplicationMockPlatformAware(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
        }

        public PrismApplicationMockPlatformAware(IPlatformInitializer platformInitializer, Page startPage) 
            : base(platformInitializer, startPage)
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();

            containerRegistry.RegisterForNavigationOnIdiom<AutowireView, AutowireViewModel>(tabletView: typeof(AutowireViewTablet));
            containerRegistry.RegisterForNavigationOnPlatform<ViewAMock, ViewModelAMock>(new Platform<ViewAMockAndroid>(AppModel.RuntimePlatform.Android));

            DependencyService.Register<IDependencyServiceMock, DependencyServiceMock>();
            ViewModelLocationProvider.Register<PartialView, PartialViewModel>();
        }
    }
}