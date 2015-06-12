using Prism.Mvvm;
using Windows.UI.Xaml;

namespace Prism.Windows.Mvvm
{
    public class ViewModelLocator
    {
        public static readonly DependencyProperty AutowireViewModelProperty =
            DependencyProperty.RegisterAttached("AutowireViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, OnAutowireViewModelChanged));

        public static bool GetAutowireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutowireViewModelProperty);
        }

        public static void SetAutowireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutowireViewModelProperty, value);
        }

        private static void OnAutowireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                ViewModelLocationProvider.AutoWireViewModelChanged(d, Bind);
        }

        private static void Bind(object view, object viewModel)
        {
            var element = view as FrameworkElement;
            if (element != null)
                element.DataContext = viewModel;
        }
    }
}
