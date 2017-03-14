using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Prism.Common
{
    public static class MvvmHelpers
    {
        public static void ViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            T viewAsT = view as T;
            if (viewAsT != null)
                action(viewAsT);
            var element = view as FrameworkElement;
            if (element != null)
            {
                var viewModelAsT = element.DataContext as T;
                if (viewModelAsT != null)
                {
                    action(viewModelAsT);
                }
            }
        }

        public static T GetImplementerFromViewOrViewModel<T>(object view) where T : class
        {
            T viewAsT = view as T;
            if (viewAsT != null)
            {
                return viewAsT;
            }

            var element = view as FrameworkElement;
            if (element != null)
            {
                var vmAsT = element.DataContext as T;
                return vmAsT;
            }

            return null;
        }
    }
}
