using Autofac;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac.Navigation
{
    public class AutofacPageNavigationService : PageNavigationService
    {
        IContainer _container;

        public AutofacPageNavigationService(IContainer container)
        {
            _container = container;
        }

        protected override Page CreatePage(string name)
        {
            return _container.ResolveNamed<Page>(name);
        }
    }
}
