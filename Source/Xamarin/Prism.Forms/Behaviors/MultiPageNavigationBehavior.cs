using Prism.Logging;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    public class MultiPageNavigationBehavior : MultiPageNavigationBehavior<Page>
    {
        public MultiPageNavigationBehavior() : base()
        {

        }

        public MultiPageNavigationBehavior( ILoggerFacade logger ) : base( logger )
        {

        }
    }
}
