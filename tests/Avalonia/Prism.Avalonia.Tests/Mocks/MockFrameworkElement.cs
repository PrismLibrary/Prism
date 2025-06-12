using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Prism.Avalonia.Tests.Mocks
{
    /// <summary>Mock Content Element</summary>
    /// <remarks>
    ///   TODO:
    ///   The Avalonia.Control's LoadedEvent and UnloadedEvent will
    ///   arrive in Avalonia v0.11.0.
    ///   Discussion: https://github.com/AvaloniaUI/Avalonia/issues/7908
    ///   PR: https://github.com/AvaloniaUI/Avalonia/pull/8277
    /// </remarks>
    public class MockFrameworkElement : Control
    {
        public void RaiseLoaded()
        {
            //// WPF: this.RaiseEvent(new RoutedEventArgs(LoadedEvent));
            this.RaiseEvent(new RoutedEventArgs());
        }

        public void RaiseUnloaded()
        {
            //// WPF: this.RaiseEvent(new RoutedEventArgs(UnloadedEvent));
            this.RaiseEvent(new RoutedEventArgs());
        }
    }
}
