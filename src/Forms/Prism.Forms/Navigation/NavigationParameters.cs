using System.Collections.Generic;
using Prism.Common;

namespace Prism.Navigation
{
    /// <summary>
    /// Represents Navigation parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to to pass object parameters during Navigation. 
    /// </remarks>
    public class NavigationParameters : ParametersBase, INavigationParameters, INavigationParametersInternal
    {
        private readonly Dictionary<string, object> _internalParameters = new Dictionary<string, object>();

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
            : base(query)
        {
        }

        #region INavigationParametersInternal
        void INavigationParametersInternal.Add(string key, object value)
        {
            _internalParameters.Add(key, value);
        }

        bool INavigationParametersInternal.ContainsKey(string key)
        {
            return _internalParameters.ContainsKey(key);
        }

        T INavigationParametersInternal.GetValue<T>(string key)
        {
            return _internalParameters.GetValue<T>(key);
        }
        #endregion
    }
}
