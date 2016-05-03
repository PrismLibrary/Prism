using Ninject;
using Prism.Common;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Ninject.Navigation
{
    public class NinjectNavigationService : PageNavigationService
    {
        IKernel _kernel;

        public NinjectNavigationService(IKernel kernel, IApplicationProvider applicationProvider)
            : base (applicationProvider)
        {
            _kernel = kernel;
        }

        protected override Page CreatePage(string name)
        {
            return _kernel.Get<object>(name) as Page;
        }
    }
}
