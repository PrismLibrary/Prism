using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Prism.Navigation
{
    public class FrameFacadeProps : DependencyObject
    {
        public static string GetCurrentNavigationPath(Frame obj)
            => (string)obj.GetValue(CurrentNavigationPathProperty);
        public static void SetCurrentNavigationPath(Frame obj, string value)
            => obj.SetValue(CurrentNavigationPathProperty, value);
        public static readonly DependencyProperty CurrentNavigationPathProperty =
            DependencyProperty.RegisterAttached("CurrentNavigationPath", typeof(string),
                typeof(FrameFacadeProps), new PropertyMetadata(string.Empty));
    }
}
