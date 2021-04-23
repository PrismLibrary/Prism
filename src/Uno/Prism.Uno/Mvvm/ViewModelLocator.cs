#if HAS_UWP
using Windows.UI.Xaml;
#elif HAS_WINUI
using Microsoft.UI.Xaml;
#endif

namespace Prism.Mvvm
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the ViewModelLocator in Prism.Mvvm.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// The AutowireViewModel attached property.
        /// </summary>
        public static DependencyProperty AutowireViewModelProperty = DependencyProperty.RegisterAttached("AutowireViewModel", typeof(bool?), typeof(ViewModelLocator), new PropertyMetadata(defaultValue: null, propertyChangedCallback: AutowireViewModelChanged));

        /// <summary>
        /// Gets the value for the <see cref="AutowireViewModelProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="AutowireViewModelProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static bool? GetAutowireViewModel(DependencyObject obj)
        {
            return (bool?)obj.GetValue(AutowireViewModelProperty);
        }

        /// <summary>
        /// Sets the <see cref="AutowireViewModelProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The value to attach.</param>
        public static void SetAutowireViewModel(DependencyObject obj, bool? value)
        {
            obj.SetValue(AutowireViewModelProperty, value);
        }

        private static void AutowireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool?)e.NewValue;
            if (value.HasValue && value.Value)
            {
                ViewModelLocationProvider.AutoWireViewModelChanged(d, Bind);
            }
        }

        /// <summary>
        /// Sets the DataContext of a View.
        /// </summary>
        /// <param name="view">The View to set the DataContext on.</param>
        /// <param name="viewModel">The object to use as the DataContext for the View.</param>
        static void Bind(object view, object viewModel)
        {
            if (view is FrameworkElement element)
                element.DataContext = viewModel;
        }
    }
}
