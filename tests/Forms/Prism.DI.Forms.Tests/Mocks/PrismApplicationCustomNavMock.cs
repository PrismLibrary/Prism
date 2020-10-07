using Prism;
using Prism.DI.Forms.Tests.Navigation;
using Prism.Ioc;
using Prism.Navigation;

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
            containerRegistry.RegisterScoped<INavigationService, CustomNavigationServiceMock>();
        }

        public INavigationService GetNavigationService() => NavigationService;
    }
}
