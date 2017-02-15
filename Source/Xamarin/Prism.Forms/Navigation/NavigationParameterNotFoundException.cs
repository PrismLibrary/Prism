using System.Collections.Generic;

namespace Prism.Navigation
{
    /// <summary>
    /// Thrown when a Prism navigation parameter does not exist.
    /// </summary>
    public class NavigationParameterNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// Creates an exception with the missing key and a default message about
        /// the missing key.
        /// </summary>
        /// <param name="missingKey">The missing navigation parameters key.</param>
        public NavigationParameterNotFoundException(string missingKey) : base("Missing navigation parameter '" + missingKey + "'.")
        {
            MissingKey = missingKey;
        }

        /// <summary>
        /// The missing navigation parameters key.
        /// </summary>
        public string MissingKey { get; }
    }
}