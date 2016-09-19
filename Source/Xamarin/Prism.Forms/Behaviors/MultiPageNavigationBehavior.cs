using Prism.Logging;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    /// <summary>
    /// Provides a MultiPageNavigationBehavior when dealing with any MultiPage&lt;Page&gt; such as a TabbedPage
    /// </summary>
    public class MultiPageNavigationBehavior : MultiPageNavigationBehavior<Page>
    {
        /// <inheritDoc />
        public MultiPageNavigationBehavior() : base()
        {

        }

        /// <inheritDoc />
        public MultiPageNavigationBehavior( ILoggerFacade logger ) : base( logger )
        {

        }
    }
}
