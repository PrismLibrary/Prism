using Windows.Foundation;
using Windows.UI.Xaml;

namespace Prism
{
    public interface IPrismApplicationEvents
    {
        event EnteredBackgroundEventHandler EnteredBackground;
        event LeavingBackgroundEventHandler LeavingBackground;
        event TypedEventHandler<PrismApplicationBase, WindowCreatedEventArgs> WindowCreated;
    }
}
