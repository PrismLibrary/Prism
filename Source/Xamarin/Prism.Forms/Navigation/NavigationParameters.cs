using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Navigation
{
    /// <summary>
    /// Represents Navigation parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to to pass object parameters during Navigation. 
    /// </remarks>
    public class NavigationParameters : Dictionary<string, object>
    {
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
                    string str = null; //key
                    string str2 = null; //value
                    if (num4 >= 0)
                    {
                        str = query.Substring(startIndex, num4 - startIndex);
                        str2 = query.Substring(num4 + 1, (i - num4) - 1);
                    }

                    if (str != null)
                        this.Add(Uri.UnescapeDataString(str), Uri.UnescapeDataString(str2));
                }
            }
        }

        /// <summary>
        /// Converts the list of key value pairs to a query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var queryBuilder = new StringBuilder();

            if (this.Count > 0)
            {
                queryBuilder.Append('?');
                var first = true;

                foreach (var kvp in this)
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

        /// <summary>
        /// Gets and types the values from the <see cref="NavigationParameters"/>.  Throws an exception if the
        /// parameter is missing or can't be cast.
        /// </summary>
        /// <typeparam name="TValueType">What type the parameter value is expected to be.</typeparam>
        /// <param name="key">The key of the parameter to parse.</param>
        /// <exception cref="NavigationParameterNotFoundException">Thrown if the key is not found.</exception>
        /// <exception cref="NavigationParameterInvalidCastException">Thrown if the key can't be cast.</exception>
        /// <returns>The value of the key from the navigation parameter.</returns>
        public TValueType GetValue<TValueType>(string key)
        {
            // Does the parameter exist.
            if (!ContainsKey(key))
            {
                throw new NavigationParameterNotFoundException(key);
            }

            // Try to cast it.
            try
            {
                return (TValueType)this[key];
            }
            catch (InvalidCastException)
            {
                throw new NavigationParameterInvalidCastException(key, typeof(TValueType), this[key].GetType());
            }
        }
    }
}
