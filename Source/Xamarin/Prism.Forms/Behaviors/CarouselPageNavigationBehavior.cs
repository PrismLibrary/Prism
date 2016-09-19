using Prism.Logging;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    /// <summary>
    /// Provides an implementation of the MultiPageNavigationBehavior when dealing with MultiPage&lt;ContentPage&gt; (CarouselPage)
    /// </summary>
    public class CarouselPageNavigationBehavior : MultiPageNavigationBehavior<ContentPage>
    {
        /// <inheritDoc />
        public CarouselPageNavigationBehavior() : base()
        {

        }

        /// <inheritDoc />
        public CarouselPageNavigationBehavior( ILoggerFacade logger ) : base( logger )
        {

        }
    }
}
