using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Navigation
{
    public static class PageRegistry
    {
        static Dictionary<string, (Type View, Type ViewModel)> _cache
            = new Dictionary<string, (Type View, Type ViewModel)>();

        public static void Register(string key, (Type View, Type ViewModel) info)
        {
            _cache.Add(key, info);
        }

        public static bool TryGetRegistration(string key, out (string Key, Type View, Type ViewModel) info)
        {
            if (_cache.ContainsKey(key))
            {
                info = (key, _cache[key].View, _cache[key].ViewModel);
                return true;
            }
            else
            {
                info = (null, null, null);
                return false;
            }
        }

        public static bool TryGetRegistration(Type view, out (string Key, Type View, Type ViewModel) info)
        {
            if (_cache.Any(x => x.Value.View == view))
            {
                var cache = _cache.FirstOrDefault(x => x.Value.View == view);
                info = (cache.Key, view, cache.Value.ViewModel);
                return true;
            }
            else if (TryGetRegistration(view.Name, out info))
            {
                return true;
            }
            else
            {
                info = (null, null, null);
                return false;
            }
        }
    }
}
