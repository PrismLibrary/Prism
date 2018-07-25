using Prism.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Navigation
{
    public class NavigationParameters : INavigationParameters, INavigationParametersInternal
    {
        public NavigationParameters(params (string Name, object Value)[] parameters)
        {
            foreach (var item in parameters)
            {
                _external.Add(item.Name, item.Value);
            }
        }

        public NavigationParameters(string query)
            : this(new Windows.Foundation.WwwFormUrlDecoder(query).Select(x => (x.Name, (object)x.Value)).ToArray())
        {
            // empty
        }

        Dictionary<string, object> _external = new Dictionary<string, object>();
        Dictionary<string, object> _internal = new Dictionary<string, object>();

        public object this[string key]
             => _external[key];

        public int Count
            => _external.Count;

        public IEnumerable<string> Keys
            => _external.Keys;

        public void Add(string key, object value)
            => _external.Add(key, value);

        public bool ContainsKey(string key)
            => _external.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => _external.GetEnumerator();

        public T GetValue<T>(string key)
        {
            return (T)Convert.ChangeType(_internal[key], typeof(T));
        }

        public IEnumerable<T> GetValues<T>(string key)
            => _external.Where(x => x.Key == key).Select(x => (T)x.Value);

        public bool TryGetValue<T>(string key, out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(_external[key], typeof(T));
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => _external.GetEnumerator();

        // internal

        void INavigationParametersInternal.Add(string key, object value)
            => _internal.Add(key, value);

        bool INavigationParametersInternal.ContainsKey(string key)
            => _internal.ContainsKey(key);

        T INavigationParametersInternal.GetValue<T>(string key)
        {
            if (_internal.TryGetValue(key, out var result))
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            else
            {
                return default(T);
            }
        }
    }

}
