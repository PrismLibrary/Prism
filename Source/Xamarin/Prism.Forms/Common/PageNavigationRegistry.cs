using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Prism.Common
{
    public static class PageNavigationRegistry
    {
        static Dictionary<string, PageNavigationInfo> _pageRegistrationCache = new Dictionary<string, PageNavigationInfo>();
        static Dictionary<Type, PageNavigationOptionsAttribute> _pageNavigationOptionsAttributeCache = new Dictionary<Type, PageNavigationOptionsAttribute>();

        public static void Register(string name, Type pageType)
        {
            var info = new PageNavigationInfo();
            info.Name = name;
            info.Type = pageType;
            info.NavigationOptions = GetPageNavigationOptions(pageType);

            if (!_pageRegistrationCache.ContainsKey(name))
                _pageRegistrationCache.Add(name, info);
        }

        public static PageNavigationInfo GetPageNavigationInfo(string name)
        {
            if (_pageRegistrationCache.ContainsKey(name))
                return _pageRegistrationCache[name];

            return null;
        }

        public static PageNavigationOptionsAttribute GetPageNavigationOptions(string name)
        {
            return GetPageNavigationInfo(name)?.NavigationOptions;
        }

        public static Type GetPageType(string name)
        {
            return GetPageNavigationInfo(name)?.Type;
        }

        static PageNavigationOptionsAttribute GetPageNavigationOptions(Type pageType)
        {
            PageNavigationOptionsAttribute attribute = null;

            if (_pageNavigationOptionsAttributeCache.ContainsKey(pageType))
                attribute = _pageNavigationOptionsAttributeCache[pageType];
            else
                attribute = pageType.GetTypeInfo().GetCustomAttribute<PageNavigationOptionsAttribute>();

            if (!_pageNavigationOptionsAttributeCache.ContainsKey(pageType))
                _pageNavigationOptionsAttributeCache.Add(pageType, attribute);

            return attribute;
        }
    }
}
