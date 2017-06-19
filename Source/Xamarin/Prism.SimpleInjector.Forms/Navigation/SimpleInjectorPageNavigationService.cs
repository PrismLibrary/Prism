using System.Runtime.CompilerServices;
using SimpleInjector;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

[assembly: InternalsVisibleTo("Prism.SimpleInjector.Forms.Tests")]
namespace Prism.SimpleInjector.Navigation
{
    /// <summary>
    /// Page navigation service for using SimpleInjector
    /// </summary>
    internal sealed class SimpleInjectorPageNavigationService : PageNavigationService, ISimpleInjectorPageNavigationService
    {
        private readonly Container _container;

        /// <summary>
        /// Create a new instance of <see cref="SimpleInjectorPageNavigationService"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="applicationProvider">An instance of <see cref="IApplicationProvider"/></param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/></param>
        /// <param name="container">An instance of <see cref="Container"/></param>
        public SimpleInjectorPageNavigationService(IApplicationProvider applicationProvider, ILoggerFacade logger, Container container)
            : base(applicationProvider, logger)
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
            var page = _container.GetInstance(segmentName) as Page;

            if (page == null)
                throw new SimpleInjectorPageNavigationException($"The requested page '{segmentName}' has not been registered.");

            return page;
        }
    }
}