using System.Windows;

namespace Prism.Wpf.Tests.Mocks
{
    public class MockFrameworkElement : FrameworkElement
    {
        public void RaiseLoaded()
        {
            this.RaiseEvent(new RoutedEventArgs(LoadedEvent));
        }

        public void RaiseUnloaded()
        {
            this.RaiseEvent(new RoutedEventArgs(UnloadedEvent));
        }
    }
}
