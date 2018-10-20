using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Navigation
{
    public class NavigationParameters : INavigationParameters, INavigationParametersInternal
    {
        public NavigationParameters()
        {
            // empty
        }

        public NavigationParameters(params (string Name, object Value)[] parameters)
            : this()
        {
            foreach (var (Name, Value) in parameters)
            {
                _external.Add(Name, Value);
            }
        }

        public NavigationParameters(string query)
            : this(string.IsNullOrWhiteSpace(query) ? Array.Empty<(string key, object value)>() : new Windows.Foundation.WwwFormUrlDecoder(query).Select(x => (x.Name, (object)x.Value)).ToArray())
        {
            // empty
        }

        public override string ToString()
        {
            var i = string.Join(",", _internal.Select(x => $"({x.Key}:{x.Value})"));
            var e = string.Join(",", _external.Select(x => $"({x.Key}:{x.Value})"));
            return $"{{internal:{i} external:{e}}}";
        }

        Dictionary<string, object> _external = new Dictionary<string, object>();
        internal Dictionary<string, object> _internal = new Dictionary<string, object>();

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
            return (T)Convert.ChangeType(_external[key], typeof(T));
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
            try
            {
                if (_internal.TryGetValue(key, out var result))
                {
                    if (result is T resultAsT)
                    {
                        return resultAsT;
                    }

                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
            catch
            {
                // ignore and return default
            }

            return default(T);
        }
    }
}
