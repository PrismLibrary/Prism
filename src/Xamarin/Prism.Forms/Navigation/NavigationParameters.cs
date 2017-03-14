using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.Navigation
{
    /// <summary>
    /// Represents Navigation parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to to pass object parameters during Navigation. 
    /// </remarks>
    public class NavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> _entries = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// Gets the number of parameters contained in the NavigationParameters
        /// </summary>
        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        /// <summary>
        /// Gets an IEnumerable containing the keys in the NavigationParameters
        /// </summary>
        public IEnumerable<string> Keys
        {
            get { return _entries.Select(x => x.Key); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
        /// </summary>
        public NavigationParameters()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationParameters"/> class with a query string.
        /// </summary>
        /// <param name="query">The query string.</param>
        public NavigationParameters(string query)
        {
            if (!String.IsNullOrWhiteSpace(query))
            {
                int num = query.Length;
                for (int i = ((query.Length > 0) && (query[0] == '?')) ? 1 : 0; i < num; i++)
                {
                    int startIndex = i;
                    int num4 = -1;
                    while (i < num)
                    {
                        char ch = query[i];
                        if (ch == '=')
                        {
                            if (num4 < 0)
                            {
                                num4 = i;
                            }
                        }
                        else if (ch == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (num4 >= 0)
                    {
                        key = query.Substring(startIndex, num4 - startIndex);
                        value = query.Substring(num4 + 1, (i - num4) - 1);
                    }
                    else
                    {
                        value = query.Substring(startIndex, i - startIndex);
                    }

                    if (key != null)
                        Add(Uri.UnescapeDataString(key), Uri.UnescapeDataString(value));
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <returns>The value for the specified key, or <see langword="null"/> if the query does not contain such a key.</returns>
        public object this[string key]
        {
            get
            {
                foreach (var kvp in _entries)
                {
                    if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                    {
                        return kvp.Value;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, object value)
        {
            _entries.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Determines whether the NavigationParameters contains the specified key
        /// </summary>
        /// <param name="key">The key to locate</param>
        public bool ContainsKey(string key)
        {
            foreach (var kvp in _entries)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a strongly typed value with the specified key.
        /// </summary>
        /// <typeparam name="T">The type to cast/convert the value to.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public T GetValue<T>(string key)
        {
            foreach (var kvp in _entries)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (kvp.Value == null)
                        return default(T);
                    else if (kvp.Value.GetType() == typeof(T))
                        return (T)kvp.Value;
                    else
                        return (T)Convert.ChangeType(kvp.Value, typeof(T));
                }
            }

            return default(T);
        }

        /// <summary>
        /// Gets a strongly typed value with the specified key.
        /// </summary>
        /// <typeparam name="T">The type to cast/convert the value to.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">Key value if such key exists.</param>
        /// <returns>True if such key exists.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            foreach (var kvp in _entries)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (kvp.Value == null)
                        value = default(T);
                    else if (kvp.Value.GetType() == typeof(T))
                        value = (T)kvp.Value;
                    else
                        value = (T)Convert.ChangeType(kvp.Value, typeof(T));

                    return true;
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Gets a strongly typed collection containing the values with the specified key.
        /// </summary>
        /// <typeparam name="T">The type to cast/convert the value to.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The collection of values.</returns>
        public IEnumerable<T> GetValues<T>(string key)
        {
            List<T> values = new List<T>();

            foreach (var kvp in _entries)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if (kvp.Value == null)
                        values.Add(default(T));
                    else if (kvp.Value.GetType() == typeof(T))
                        values.Add((T)kvp.Value);
                    else
                        values.Add((T)Convert.ChangeType(kvp.Value, typeof(T)));
                }
            }

            return values.AsEnumerable();
        }

        /// <summary>
        /// Converts the list of key value pairs to a query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var queryBuilder = new StringBuilder();

            if (_entries.Count > 0)
            {
                queryBuilder.Append('?');
                var first = true;

                foreach (var kvp in _entries)
                {
                    if (!first)
                    {
                        queryBuilder.Append('&');
                    }
                    else
                    {
                        first = false;
                    }

                    queryBuilder.Append(Uri.EscapeDataString(kvp.Key));
                    queryBuilder.Append('=');
                    queryBuilder.Append(Uri.EscapeDataString(kvp.Value.ToString()));
                }
            }

            return queryBuilder.ToString();
        }
    }
}
