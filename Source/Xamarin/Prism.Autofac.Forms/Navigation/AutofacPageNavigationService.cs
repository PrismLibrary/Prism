using System;
using Autofac;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Autofac.Navigation
{
    /// <summary>
    /// Page navigation service for using Autofac
    /// </summary>
    public class AutofacPageNavigationService : PageNavigationService
    {
        readonly IContainer _container;

        /// <summary>
        /// Create a new instance of <see cref="AutofacPageNavigationService"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="applicationProvider">An instance of <see cref="IApplicationProvider"/></param>
        /// <param name="container">An instance of <see cref="IContainer"/></param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/></param>
        public AutofacPageNavigationService(IContainer container, IApplicationProvider applicationProvider, ILoggerFacade logger)
            : base(applicationProvider, logger)
        {
            _container = container;
        }

        /// <summary>
        /// Resolve a <see cref="Page"/> from <see cref="_container"/> for <paramref name="segmentName"/>
        /// </summary>
        /// <param name="segmentName">Page to resolve</param>
        /// <returns>A <see cref="Page"/></returns>
        protected override Page CreatePage(string name)
        {
            if (!_container.IsRegisteredWithName<Page>(name))
                throw new NullReferenceException($"The requested page '{name}' has not been registered.");

            return _container.ResolveNamed<Page>(name);
        }
    }
}
