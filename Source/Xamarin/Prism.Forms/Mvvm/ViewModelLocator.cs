using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Mvvm
{
    public static class ViewModelLocator
    {
        public static readonly BindableProperty AutowireViewModelProperty = BindableProperty.CreateAttached<BindableObject, bool>(
            p => ViewModelLocator.GetAutowireViewModel(p),
            default(bool),
            BindingMode.OneWay,
            null,
            OnAutowireViewModelChanged,
            null,
            null,
            null);

        public static bool GetAutowireViewModel(BindableObject bo)
        {
            return (bool)bo.GetValue(ViewModelLocator.AutowireViewModelProperty);
        }
        public static void SetAutowireViewModel(BindableObject bo, bool value)
        {
            bo.SetValue(ViewModelLocator.AutowireViewModelProperty, value);
        }

        public static void OnAutowireViewModelChanged(BindableObject bo, bool oldValue, bool newValue)
        {
            if (newValue)
                ViewModelLocationProvider.AutoWireViewModelChanged(bo, Bind);
        }

        /// <summary>
        /// Sets the DataContext of a View
        /// </summary>
        /// <param name="view">The View to set the DataContext on</param>
        /// <param name="dataContext">The object to use as the DataContext for the View</param>
        static void Bind(object view, object viewModel)
        {
            BindableObject element = view as BindableObject;
            if (element != null)
                element.BindingContext = viewModel;

            if (view is Page)
            {
                var iPageAware = viewModel as IPageAware;
                if (iPageAware != null)
                    iPageAware.Page = view;
            }
        }
    }
}
