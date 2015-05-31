using Microsoft.Practices.ServiceLocation;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Mvvm
{
    public static class ViewModelLocator
    {
        public static readonly BindableProperty AutowireViewModelProperty =
            BindableProperty.CreateAttached("AutowireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutowireViewModelChanged);

        public static bool GetAutowireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutowireViewModelProperty);
        }
        public static void SetAutowireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutowireViewModelProperty, value);
        }

        private static void OnAutowireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bool)newValue)
                ViewModelLocationProvider.AutoWireViewModelChanged(bindable, Bind);
        }

        /// <summary>
        /// Sets the DataContext of a View
        /// </summary>
        /// <param name="view">The View to set the DataContext on</param>
        /// <param name="viewModel">The object to use as the DataContext for the View</param>
        static void Bind(object view, object viewModel)
        {
            BindableObject element = view as BindableObject;
            if (element != null)
                element.BindingContext = viewModel;
        }
    }
}
