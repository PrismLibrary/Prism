using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Prism.Common
{
    /// <summary>
    /// Base class for All Parameters
    /// </summary>
    public abstract class ParametersBase : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> _entries = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// Default base constructor
        /// </summary>
        protected ParametersBase()
        {
        }

        /// <summary>
        /// Single parameter constuctor that turns the query into a Key, Value pair
        /// <see cref="Add(string, object)"/>
        /// <see cref="_entries"/>
        /// </summary>
        /// <param name="query"></param>
        protected ParametersBase(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
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
        /// Searches for parameter value
        /// </summary>
        /// <value>
        /// Returns value if key exists; otherwise returns null
        /// </value>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                foreach(var entry in _entries)
                {
                    if(string.Compare(entry.Key, key, StringComparison.Ordinal) == 0)
                    {
                        return entry.Value;
                    }
                }

                return null;
            }
        }
        /// <summary>
        /// Returns count of parameters in <see cref="_entries" /> collection/>
        /// </summary>
        public int Count => _entries.Count;

        public IEnumerable<string> Keys => 
            _entries.Select(x => x.Key);

        public void Add(string key, object value) =>
            _entries.Add(new KeyValuePair<string, object>(key, value));

        public bool ContainsKey(string key) =>
            _entries.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() =>
            _entries.GetEnumerator();

        public T GetValue<T>(string key) => 
            _entries.GetValue<T>(key);

        public IEnumerable<T> GetValues<T>(string key) =>
            _entries.GetValues<T>(key);

        public bool TryGetValue<T>(string key, out T value) =>
            _entries.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

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
                    queryBuilder.Append(Uri.EscapeDataString(kvp.Value != null ? kvp.Value.ToString() : ""));
                }
            }

            return queryBuilder.ToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void FromParameters(IEnumerable<KeyValuePair<string, object>> parameters) =>
            _entries.AddRange(parameters);
    }
}
