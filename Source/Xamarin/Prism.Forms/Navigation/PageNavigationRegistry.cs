using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Prism.Navigation
{
    public static class PageNavigationRegistry
    {
        static Dictionary<string, PageNavigationInfo> _pageRegistrationCache = new Dictionary<string, PageNavigationInfo>();

        public static void Register(string name, Type pageType)
        {
            var info = new PageNavigationInfo
            {
                Name = name,
                Type = pageType
            };

            if (!_pageRegistrationCache.ContainsKey(name))
                _pageRegistrationCache.Add(name, info);
        }

        public static PageNavigationInfo GetPageNavigationInfo(string name)
        {
            if (_pageRegistrationCache.ContainsKey(name))
                return _pageRegistrationCache[name];

            return null;
        }

        public static PageNavigationInfo GetPageNavigationInfo(Type pageType)
        {
            foreach (var item in _pageRegistrationCache)
            {
                if (item.Value.Type == pageType)
                    return item.Value;
            }

            return null;
        }

        public static Type GetPageType(string name)
        {
            return GetPageNavigationInfo(name)?.Type;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearRegistrationCache()
        {
            _pageRegistrationCache.Clear();
        }
    }
}
