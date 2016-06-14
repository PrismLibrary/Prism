using Autofac;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac.Navigation
{
    public class AutofacPageNavigationService : PageNavigationService
    {
        readonly IContainer _container;

        public AutofacPageNavigationService(IContainer container, IApplicationProvider applicationProvider, ILoggerFacade logger)
            : base(applicationProvider, logger)
        {
            _container = container;
        }

        protected override Page CreatePage(string name)
        {
            return _container.ResolveNamed<Page>(name);
        }
    }
}
