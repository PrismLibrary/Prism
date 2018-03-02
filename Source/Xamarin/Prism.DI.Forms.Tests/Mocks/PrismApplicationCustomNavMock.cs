using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.DI.Forms.Tests.Navigation;

namespace Prism.DI.Forms.Tests
{
    public class PrismApplicationCustomNavMock : PrismApplicationMock
    {
        public PrismApplicationCustomNavMock(IPlatformInitializer initializer)
            : base(initializer)
        {
            
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
#if DryIoc
            containerRegistry.Register<INavigationService, CustomNavigationServiceMock>();
#endif
            containerRegistry.Register<INavigationService, CustomNavigationServiceMock>(NavigationServiceName);
        }

        public INavigationService GetNavigationService() => NavigationService;
    }
}