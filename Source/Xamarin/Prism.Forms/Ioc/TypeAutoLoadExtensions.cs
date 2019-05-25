using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Prism.Ioc
{
    internal static class IContainerRegistryAutoLoadExtensions
    {
        public static void AutoRegisterViews(this Type type, IContainerRegistry containerRegistry, Func<Type, string> getNavigationSegmentName)
        {
            if (!type.GetCustomAttributes().Any(a => a is AutoRegisterForNavigationAttribute)) return;

            var regAttr = type.GetCustomAttribute<AutoRegisterForNavigationAttribute>();
            var assembly = type.Assembly;

            var viewTypes = assembly.ExportedTypes.Where(t => t.IsSubclassOf(typeof(Page)));
            RegisterViewsAutomatically(containerRegistry, viewTypes, getNavigationSegmentName);
        }

        private static void RegisterViewsAutomatically(IContainerRegistry containerRegistry, IEnumerable<Type> viewTypes, Func<Type, string> getNavigationSegmentName)
        {
            foreach (var viewType in viewTypes)
            {
                RegisterView(containerRegistry, viewType, getNavigationSegmentName);
            }

            RegisterView(containerRegistry, typeof(NavigationPage), getNavigationSegmentName, true);
            RegisterView(containerRegistry, typeof(TabbedPage), getNavigationSegmentName, true);
        }

        private static void RegisterView(IContainerRegistry containerRegistry, Type viewType, Func<Type, string> getNavigationSegmentName, bool checkIfRegistered = false)
        {
            var name = getNavigationSegmentName(viewType);

            if(!checkIfRegistered || containerRegistry.IsRegistered<object>(name))
            {
                containerRegistry.RegisterForNavigation(viewType, name);
            }
        }
    }
}
