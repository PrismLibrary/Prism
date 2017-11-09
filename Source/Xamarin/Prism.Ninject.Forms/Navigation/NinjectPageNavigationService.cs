using Ninject;
using Prism.Behaviors;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Ninject.Navigation
{
    public class NinjectPageNavigationService : PageNavigationService
    {
        IKernel _kernel;

        public NinjectPageNavigationService(IKernel kernel, IApplicationProvider applicationProvider, IPageBehaviorFactory pageBehaviorFactory, ILoggerFacade logger)
            : base (applicationProvider, pageBehaviorFactory, logger)
        {
            _kernel = kernel;
        }

        protected override Page CreatePage(string name)
        {
            return _kernel.Get<object>(name) as Page;
        }
    }
}
