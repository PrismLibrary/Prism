using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Prism.Navigation
{

    public static class NavigationServiceLocator
    {
        private static Func<Frame, CoreWindow, INavigationService> _navigationServiceResolver;
        private static Dictionary<Frame, INavigationService> Instances = new Dictionary<Frame, INavigationService>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService GetNavigationService(Page page)
        {
            if(!Instances.ContainsKey(page.Frame))
            {
                Instances[page.Frame] = _navigationServiceResolver(page.Frame, CoreWindow.GetForCurrentThread());
            }

            return Instances[page.Frame];
        }

        public static void RegisterNavigationDelegate(Func<Frame, CoreWindow, INavigationService> func) =>
            _navigationServiceResolver = func;
    }
}
