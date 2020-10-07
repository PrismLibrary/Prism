using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;

namespace Prism.DI.Forms.Tests.Navigation
{
    public class CustomNavigationServiceMock : PageNavigationService
    {
        public CustomNavigationServiceMock(IContainerExtension container, IApplicationProvider applicationProvider, IPageBehaviorFactory pageBehaviorFactory)
            : base(container, applicationProvider, pageBehaviorFactory)
        {
        }
    }
}
