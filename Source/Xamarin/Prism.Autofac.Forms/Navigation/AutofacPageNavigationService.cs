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
        readonly IComponentContext _context;

        /// <summary>
        /// Create a new instance of <see cref="AutofacPageNavigationService"/> with <paramref name="context"/>
        /// </summary>
        /// <param name="applicationProvider">An instance of <see cref="IApplicationProvider"/></param>
        /// <param name="context">An instance of <see cref="IComponentContext"/></param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/></param>
        public AutofacPageNavigationService(IComponentContext context, IApplicationProvider applicationProvider, ILoggerFacade logger)
            : base(applicationProvider, logger)
        {
            _context = context;
        }

        /// <summary>
        /// Resolve a <see cref="Page"/> from <see cref="_context"/> for <paramref name="segmentName"/>
        /// </summary>
        /// <param name="segmentName">Page to resolve</param>
        /// <returns>A <see cref="Page"/></returns>
        protected override Page CreatePage(string segmentName)
        {
            if (!_context.IsRegisteredWithName<Page>(segmentName))
                throw new NullReferenceException($"The requested page '{segmentName}' has not been registered.");

            return _context.ResolveNamed<Page>(segmentName);
        }
    }
}
