using System.Windows;

namespace Prism.Common
{
    public static class WindowStartupLocationBehavior
    {
        public static readonly DependencyProperty WindowStartupLocationProperty;

        public static WindowStartupLocation GetWindowStartupLocation(DependencyObject obj)
        {
            return (WindowStartupLocation)obj.GetValue(WindowStartupLocationProperty);
        }

        public static void SetWindowStartupLocation(DependencyObject obj, WindowStartupLocation value)
        {
            obj.SetValue(WindowStartupLocationProperty, value);
        }

        static WindowStartupLocationBehavior()
        {
            WindowStartupLocationProperty = DependencyProperty.RegisterAttached("WindowStartupLocation", typeof(WindowStartupLocation), typeof(WindowStartupLocationBehavior), new UIPropertyMetadata(WindowStartupLocation.CenterOwner, OnWindowStartupLocationChanged));
        }

        private static void OnWindowStartupLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Window window)
                window.WindowStartupLocation = GetWindowStartupLocation(window);
        }
    }
}
