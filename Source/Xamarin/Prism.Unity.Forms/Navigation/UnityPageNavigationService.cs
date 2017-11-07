using Unity;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Unity.Navigation
{
    public class UnityPageNavigationService : PageNavigationService
    {
        IUnityContainer _container;

        public UnityPageNavigationService(IUnityContainer container, IApplicationProvider applicationProvider, ILoggerFacade logger)
            : base(applicationProvider, logger)
        {
            _container = container;
        }

        protected override Page CreatePage(string name)
        {
            return _container.Resolve<object>(name) as Page;
        }
    }
}
