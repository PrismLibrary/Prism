using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prism.Avalonia.Tests.Mocks
{
    /// <summary>Mock Framework Content Element</summary>
    /// <remarks>
    ///   The Avalonia.Control's LoadedEvent and UnloadedEvent will
    ///   arrive in Avalonia v0.11.0.
    ///   Discussion: https://github.com/AvaloniaUI/Avalonia/issues/7908
    ///   PR: https://github.com/AvaloniaUI/Avalonia/pull/8277
    /// </remarks>
    public class MockFrameworkContentElement : Control
    {
        public void RaiseLoaded()
        {
            ////this.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            this.RaiseEvent(new RoutedEventArgs());
        }

        public void RaiseUnloaded()
        {
            //// this.RaiseEvent(new RoutedEventArgs(UnloadedEvent));
            this.RaiseEvent(new RoutedEventArgs());
        }
    }
}
