using Grace.DependencyInjection;
using Grace.DependencyInjection.Exceptions;
using Prism.Behaviors;
using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Prism.Grace.Navigation
{
    /// <summary>
    /// Page navigation service for using DryIoc
    /// </summary>
    public class GracePageNavigationService : PageNavigationService
    {
        private readonly DependencyInjectionContainer _container;

        /// <summary>
        /// Create a new instance of <see cref="GracePageNavigationService"/> with <paramref name="container"/>
        /// </summary>
        /// <param name="applicationProvider">An instance of <see cref="IApplicationProvider"/></param>
        /// <param name="container">An instance of <see cref="DependencyInjectionContainer"/></param>
        /// <param name="logger">An instance of <see cref="ILoggerFacade"/></param>
        public GracePageNavigationService(IApplicationProvider applicationProvider, DependencyInjectionContainer container, IPageBehaviorFactory pageBehaviorFactory, ILoggerFacade logger)
            : base(applicationProvider, pageBehaviorFactory, logger)
        {
            _container = container;
        }

        /// <summary>
        /// Resolve a <see cref="Page"/> from <see cref="_container"/> for <paramref name="segmentName"/>
        /// </summary>
        /// <param name="name">Page to resolve</param>
        /// <returns>A <see cref="Page"/></returns>
        protected override Page CreatePage(string name)
        {
            try
            {
                return _container.Locate<object>(withKey: name) as Page;
            }
            catch(LocateException locateException)
            {
                throw new NullReferenceException("Could not locate Page with name: " + name, locateException);
            }
        }
    }
}