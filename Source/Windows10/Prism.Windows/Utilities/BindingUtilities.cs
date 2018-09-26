using System;
using System.Reflection;
using Windows.UI.Xaml.Controls;

namespace Prism.Utilities
{
    public static class BindingUtilities
    {
        public static void UpdateBindings(Page page)
        {
            if (page == null)
            {
                return;
            }
            var field = page.GetType().GetTypeInfo().GetDeclaredField("Bindings");
            var bindings = field?.GetValue(page);
            var update = bindings?.GetType().GetRuntimeMethod("Update", new Type[] { });
            update?.Invoke(bindings, null);
        }

        public static void InitializeBindings(Page page)
        {
            if (page == null)
            {
                return;
            }
            var field = page.GetType().GetTypeInfo().GetDeclaredField("Bindings");
            var bindings = field?.GetValue(page);
            var update = bindings?.GetType().GetRuntimeMethod("Initialize", new Type[] { });
            update?.Invoke(bindings, null);
        }

        public static void StopTrackingBindings(Page page)
        {
            if (page == null)
            {
                return;
            }
            var field = page.GetType().GetTypeInfo().GetDeclaredField("Bindings");
            var bindings = field?.GetValue(page);
            var update = bindings?.GetType().GetRuntimeMethod("StopTracking", new Type[] { });
            update?.Invoke(bindings, null);
        }
    }
}
