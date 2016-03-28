using DryIoc;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.DryIoc.Navigation
{
    /// <summary>
    /// Page navigation service for using DryIoc
    /// </summary>
    public class DryIocPageNavigationService : PageNavigationService
    {
        private readonly IContainer _container;

        /// <summary>
        /// Create a new instance of <see cref="DryIocPageNavigationService"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="container">An instance of <see cref="IContainer"/></param>
        public DryIocPageNavigationService(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolve a <see cref="Page"/> from <see cref="_container"/> for <paramref name="segmentName"/>
        /// </summary>
        /// <param name="segmentName">Page to resolve</param>
        /// <returns>A <see cref="Page"/></returns>
        protected override Page CreatePage(string segmentName)
        {
            return _container.Resolve<Page>(segmentName, IfUnresolved.ReturnDefault);
        }
    }
}