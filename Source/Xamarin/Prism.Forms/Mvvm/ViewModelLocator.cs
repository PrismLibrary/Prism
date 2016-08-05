using Xamarin.Forms;

namespace Prism.Mvvm
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the <see cref="Prism.Mvvm.ViewModelLocationProvider"/>.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// Instructs Prism whether or not to automatically create an instance of a ViewModel using a convention, and assign the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        public static readonly BindableProperty AutowireViewModelProperty =
            BindableProperty.CreateAttached("AutowireViewModel", typeof(bool?), typeof(ViewModelLocator), null, propertyChanged: OnAutowireViewModelChanged);

        /// <summary>
        /// Gets the AutowireViewModel property value.
        /// </summary>
        /// <param name="bindable"></param>
        /// <returns></returns>
        public static bool? GetAutowireViewModel(BindableObject bindable)
        {
            return (bool?)bindable.GetValue(ViewModelLocator.AutowireViewModelProperty);
        }

        /// <summary>
        /// Sets the AutowireViewModel property value.  If <c>true</c>, creates an instance of a ViewModel using a convention, and sets the associated View's <see cref="Xamarin.Forms.BindableObject.BindingContext"/> to that instance.
        /// </summary>
        /// <param name="bindable"></param>
        /// <param name="value"></param>
        public static void SetAutowireViewModel(BindableObject bindable, bool? value)
        {
            bindable.SetValue(ViewModelLocator.AutowireViewModelProperty, value);
        }

        private static void OnAutowireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            bool? bNewValue = (bool?)newValue;
            if (bNewValue.HasValue && bNewValue.Value)
                ViewModelLocationProvider.AutoWireViewModelChanged(bindable, Bind);
        }

        /// <summary>
        /// Sets the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> of a View
        /// </summary>
        /// <param name="view">The View to set the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> on</param>
        /// <param name="viewModel">The object to use as the <see cref="Xamarin.Forms.BindableObject.BindingContext"/> for the View</param>
        static void Bind(object view, object viewModel)
        {
            BindableObject element = view as BindableObject;
            if (element != null)
                element.BindingContext = viewModel;
        }
    }
}
