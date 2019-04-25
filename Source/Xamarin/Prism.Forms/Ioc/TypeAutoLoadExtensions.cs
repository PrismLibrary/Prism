using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Prism.Ioc
{
    internal static class IContainerRegistryAutoLoadExtensions
    {
        public static void AutoRegisterViews(this Type type, IContainerRegistry containerRegistry)
        {
            if (!type.GetCustomAttributes().Any(a => a is AutoRegisterForNavigationAttribute)) return;

            var regAttr = type.GetCustomAttribute<AutoRegisterForNavigationAttribute>();
            var assembly = type.Assembly;

            if (regAttr.Automatic)
            {
                var viewTypes = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(Page)));
                RegisterViewsAutomatically(containerRegistry, viewTypes);
            }
            else
            {
                RegisterViewsByAttribute(containerRegistry, assembly);
            }
        }

        private static void RegisterViewsAutomatically(IContainerRegistry containerRegistry, IEnumerable<Type> viewTypes)
        {
            foreach (var viewType in viewTypes)
            {
                containerRegistry.RegisterForNavigation(viewType, viewType.Name);
            }

            if (!containerRegistry.IsRegistered<object>(nameof(NavigationPage)))
            {
                containerRegistry.RegisterForNavigation<NavigationPage>();
            }

            if (!containerRegistry.IsRegistered<object>(nameof(TabbedPage)))
            {
                containerRegistry.RegisterForNavigation<TabbedPage>();
            }
        }

        private static void RegisterViewsByAttribute(IContainerRegistry containerRegistry, Assembly assembly)
        {
            var attrs = assembly.GetCustomAttributes<RegisterForNavigationAttribute>();
            foreach (var attr in attrs)
            {
                RegisterViewByAttribute(containerRegistry, attr);
            }

            var viewTypes = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(Page))
                && t.GetCustomAttributes<RegisterForNavigationAttribute>().Any());
            foreach (var viewType in viewTypes)
            {
                var attr = viewType.GetCustomAttributes<RegisterForNavigationAttribute>()
                    .FirstOrDefault(a => a.RuntimePlatform.ToString() == Device.RuntimePlatform || a.RuntimePlatform is null);
                attr.ViewType = viewType;
                RegisterViewByAttribute(containerRegistry, attr);
            }
        }

        private static void RegisterViewByAttribute(IContainerRegistry containerRegistry, RegisterForNavigationAttribute attr)
        {
            if (attr.ViewType is null)
                throw new Exception($"Cannot auto register View. No ViewType was specified. Name: '{attr.Name}'. ViewModelType: '{attr.ViewModelType?.Name}'");

            if (attr.RuntimePlatform != null && attr.RuntimePlatform.ToString() != Device.RuntimePlatform) return;

            var name = attr.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = attr.ViewType.Name;
            }

            containerRegistry.RegisterForNavigation(attr.ViewType, name);

            if (attr.ViewModelType != null)
            {
                ViewModelLocationProvider.Register(attr.ViewType.Name, attr.ViewModelType);
            }
        }
    }
}
