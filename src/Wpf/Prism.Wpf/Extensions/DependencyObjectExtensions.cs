using System;
using System.Collections.Generic;
using System.Text;

#if HAS_WINUI
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Prism
{
    internal static class DependencyObjectExtensions
    {
        public static bool CheckAccess(this DependencyObject instance)
#if __WASM__
            // This needs to evolve, once threading is supported
            // See https://github.com/unoplatform/uno/issues/2302
            => System.Threading.Thread.CurrentThread.ManagedThreadId == 1;
#elif HAS_WINUI
            => instance.Dispatcher.HasThreadAccess;
#else
            => instance.CheckAccess();
#endif

        public static BindingExpression GetBinding(this FrameworkElement instance, DependencyProperty property)
            => instance.GetBindingExpression(property);
    }
}
