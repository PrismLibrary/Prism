using Prism.Logging;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    public class CarouselPageNavigationBehavior : MultiPageNavigationBehavior<ContentPage>
    {
        public CarouselPageNavigationBehavior() : base()
        {

        }

        public CarouselPageNavigationBehavior( ILoggerFacade logger ) : base( logger )
        {

        }
    }
}
