using Autofac;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac.Navigation
{
    public class AutofacPageNavigationService : PageNavigationService
    {
        readonly IComponentContext _context;

        public AutofacPageNavigationService(IComponentContext container, IApplicationProvider applicationProvider, ILoggerFacade logger)
            : base(applicationProvider, logger)
        {
            _context = container;
        }

        protected override Page CreatePage(string name)
        {
            return _context.ResolveNamed<Page>(name);
        }
    }
}
