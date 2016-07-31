using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    //TODO: Once UWP and Xamarin.Forms navigation has been implemented and stablized, see if we can move this class to the core Prism assembly.  Right now each platform has it's own implementation
    /// <summary>
    /// Represents Navigation parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to to pass object parameters during Navigation. 
    /// </remarks>
    public class NavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> entries = new List<KeyValuePair<string, object>>();

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
            if (query != null)
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
                    string str = null;
                    string str2 = null;
                    if (num4 >= 0)
                    {
                        str = query.Substring(startIndex, num4 - startIndex);
                        str2 = query.Substring(num4 + 1, (i - num4) - 1);
                    }
                    else
                    {
                        str2 = query.Substring(startIndex, i - startIndex);
                    }

                    this.Add(str != null ? Uri.UnescapeDataString(str) : null, Uri.UnescapeDataString(str2));
                    if ((i == (num - 1)) && (query[i] == '&'))
                    {
                        this.Add(null, "");
                    }
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
                foreach (var kvp in this.entries)
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
            return this.entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds the specified key and value.
        /// </summary>
        /// <param name="key">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, object value)
        {
            this.entries.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Converts the list of key value pairs to a query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var queryBuilder = new StringBuilder();

            if (this.entries.Count > 0)
            {
                queryBuilder.Append('?');
                var first = true;

                foreach (var kvp in this.entries)
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
