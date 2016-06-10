using Autofac;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac.Navigation
{
    public class AutofacPageNavigationService : PageNavigationService
    {
        readonly IComponentContext _context;

        public AutofacPageNavigationService(IComponentContext container)
        {
            _context = container;
        }

        protected override Page CreatePage(string name)
        {
            return _context.ResolveNamed<Page>(name);
        }
    }
}
