#if HAS_WINUI
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// This class contains <see cref="IDialogWindow"/> attached properties.
    /// </summary>
    public class Dialog
    {
        /// <summary>
        /// Identifies the WindowStyle attached property.
        /// </summary>
        /// <remarks>
        /// This attached property is used to specify the style of a <see cref="IDialogWindow"/>.
        /// </remarks>
        public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.RegisterAttached("WindowStyle", typeof(Style), typeof(Dialog), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value for the <see cref="WindowStyleProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowStyleProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static Style GetWindowStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(WindowStyleProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowStyleProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The Style to attach.</param>
        public static void SetWindowStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(WindowStyleProperty, value);
        }

#if !HAS_WINUI
        /// <summary>
        /// Identifies the WindowStartupLocation attached property.
        /// </summary>
        /// <remarks>
        /// This attached property is used to specify the startup location of a <see cref="IDialogWindow"/>.
        /// </remarks>
        public static readonly DependencyProperty WindowStartupLocationProperty =
            DependencyProperty.RegisterAttached("WindowStartupLocation", typeof(WindowStartupLocation), typeof(Dialog), new UIPropertyMetadata(OnWindowStartupLocationChanged));

        /// <summary>
        /// Gets the value for the <see cref="WindowStartupLocationProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="WindowStartupLocationProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static WindowStartupLocation GetWindowStartupLocation(DependencyObject obj)
        {
            return (WindowStartupLocation)obj.GetValue(WindowStartupLocationProperty);
        }

        /// <summary>
        /// Sets the <see cref="WindowStartupLocationProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The WindowStartupLocation to attach.</param>
        public static void SetWindowStartupLocation(DependencyObject obj, WindowStartupLocation value)
        {
            obj.SetValue(WindowStartupLocationProperty, value);
        }

        private static void OnWindowStartupLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Window window)
                window.WindowStartupLocation = (WindowStartupLocation)e.NewValue;
        }
#endif
    }
}
