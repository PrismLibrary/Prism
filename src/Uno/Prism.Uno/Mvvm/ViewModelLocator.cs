namespace Prism.Mvvm
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the ViewModelLocator in Prism.Mvvm.
    /// </summary>
    public static partial class ViewModelLocator
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
                Autowire(d, true);
            }
        }
    }
}
