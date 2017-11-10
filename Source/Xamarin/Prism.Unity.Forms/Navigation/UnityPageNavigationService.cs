using Unity;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;
using Prism.Behaviors;

namespace Prism.Unity.Navigation
{
    public class UnityPageNavigationService : PageNavigationService
    {
        IUnityContainer _container;

        public UnityPageNavigationService(IUnityContainer container, IApplicationProvider applicationProvider, IPageBehaviorFactory pageBehaviorFactory, ILoggerFacade logger)
            : base(applicationProvider, pageBehaviorFactory, logger)
        {
            _container = container;
        }

        protected override Page CreatePage(string name)
        {
            return _container.Resolve<object>(name) as Page;
        }
    }
}
