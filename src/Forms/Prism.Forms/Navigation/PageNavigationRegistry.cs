using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Prism.Navigation
{
    /// <summary>
    /// Maintains page navigation registrations.
    /// </summary>
    public static class PageNavigationRegistry
    {
        static Dictionary<string, PageNavigationInfo> _pageRegistrationCache = new Dictionary<string, PageNavigationInfo>();

        /// <summary>
        /// Registers a Page for navigation.
        /// </summary>
        /// <param name="name">The unique name to register with the Page.</param>
        /// <param name="pageType">The type of Page to registe.r</param>
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

        /// <summary>
        /// Gets the <see cref="PageNavigationInfo"/> for a given registration name.
        /// </summary>
        /// <param name="name">The registration name.</param>
        /// <returns>The <see cref="PageNavigationInfo"/> instance for the given name if registered, null otherwise.</returns>
        public static PageNavigationInfo GetPageNavigationInfo(string name)
        {
            if (_pageRegistrationCache.ContainsKey(name))
                return _pageRegistrationCache[name];

            return null;
        }

        /// <summary>
        /// Gets the <see cref="PageNavigationInfo"/> for a given page type.
        /// </summary>
        /// <param name="pageType">The registration's type.</param>
        /// <returns>The first <see cref="PageNavigationInfo"/> instance for the given type if registered, null otherwise.</returns>
        public static PageNavigationInfo GetPageNavigationInfo(Type pageType)
        {
            foreach (var item in _pageRegistrationCache)
            {
                if (item.Value.Type == pageType)
                    return item.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets the type for a given registration name.
        /// </summary>
        /// <param name="name">The registration name.</param>
        /// <returns>The type of the registration for the given name if registered, null otherwise</returns>
        public static Type GetPageType(string name)
        {
            return GetPageNavigationInfo(name)?.Type;
        }

        /// <summary>
        /// Clears the page registration cache.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearRegistrationCache()
        {
            _pageRegistrationCache.Clear();
        }
    }
}
