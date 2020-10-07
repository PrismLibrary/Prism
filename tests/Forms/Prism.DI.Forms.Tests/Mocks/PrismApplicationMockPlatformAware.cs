using Prism.DI.Forms.Tests.Mocks.Modules;
using Prism.DI.Forms.Tests.Mocks.Services;
using Prism.DI.Forms.Tests.Mocks.ViewModels;
using Prism.DI.Forms.Tests.Mocks.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;
using Prism.Mvvm;
using System;
#if DryIoc
using Prism.DryIoc;
using DryIoc;
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

        public PrismApplicationMockPlatformAware(IPlatformInitializer platformInitializer, Func<Page> startPage)
            : base(platformInitializer, startPage)
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();

            containerRegistry.RegisterForNavigationOnIdiom<AutowireView, AutowireViewModel>(tabletView: typeof(AutowireViewTablet));
            containerRegistry.RegisterForNavigationOnPlatform<ViewAMock, ViewModelAMock>(new Platform<ViewAMockAndroid>(AppModel.RuntimePlatform.Android));

            ViewModelLocationProvider.Register<PartialView, PartialViewModel>();
        }
    }
}
