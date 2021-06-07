using System;
using Prism.Navigation;

namespace Prism.Navigation
{
    /// <summary>
    /// Represents page registration information
    /// </summary>
    public class PageNavigationInfo
    {
        /// <summary>
        /// The unique name to registered Page
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the registered Page
        /// </summary>
        public Type Type { get; set; }
    }
}
