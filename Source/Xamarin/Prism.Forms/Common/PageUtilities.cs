using System;
using Xamarin.Forms;

namespace Prism.Common
{
    public static class PageUtilities
    {
        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            T viewAsT = view as T;
            if (viewAsT != null)
                action(viewAsT);

            var element = view as BindableObject;
            if (element != null)
            {
                var viewModelAsT = element.BindingContext as T;
                if (viewModelAsT != null)
                {
                    action(viewModelAsT);
                }
            }
        }

        public static void DisposePage(Page page)
        {
            var viewModel = page.BindingContext as IDisposable;
            if (viewModel != null)
                viewModel.Dispose();

            page.Behaviors.Clear();
            page.BindingContext = null;

            var disposablePage = page as IDisposable;
            if (disposablePage != null)
                disposablePage.Dispose();
        }
    }
}
