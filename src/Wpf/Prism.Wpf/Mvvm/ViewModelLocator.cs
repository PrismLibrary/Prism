using System.ComponentModel;

namespace Prism.Mvvm
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the ViewModelLocator in Prism.Mvvm.
    /// </summary>
    public static partial class ViewModelLocator
    {
#if AVALONIA
        static ViewModelLocator()
        {
            // Bind AutoWireViewModelProperty.Changed to its callback
            AutoWireViewModelProperty.Changed.Subscribe(args => AutoWireViewModelChanged(args?.Sender, args));
        }

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static AvaloniaProperty AutoWireViewModelProperty =
            AvaloniaProperty.RegisterAttached<Control, bool?>(
                name: "AutoWireViewModel",
                ownerType: typeof(ViewModelLocator),
                defaultValue: null);
#else
        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool?), typeof(ViewModelLocator), new PropertyMetadata(defaultValue: null, propertyChangedCallback: AutoWireViewModelChanged));
#endif

        /// <summary>
        /// Gets the value for the <see cref="AutoWireViewModelProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <returns>The <see cref="AutoWireViewModelProperty"/> attached to the <paramref name="obj"/> element.</returns>
        public static bool? GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool?)obj.GetValue(AutoWireViewModelProperty);
        }

        /// <summary>
        /// Sets the <see cref="AutoWireViewModelProperty"/> attached property.
        /// </summary>
        /// <param name="obj">The target element.</param>
        /// <param name="value">The value to attach.</param>
        public static void SetAutoWireViewModel(DependencyObject obj, bool? value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
#if AVALONIA
            if (!Design.IsDesignMode)
#else
            if (!DesignerProperties.GetIsInDesignMode(d))
#endif
            {
                var value = (bool?)e.NewValue;
                if (value.HasValue && value.Value)
                {
                    Autowire(d, true);
                }
            }
        }
    }
}
