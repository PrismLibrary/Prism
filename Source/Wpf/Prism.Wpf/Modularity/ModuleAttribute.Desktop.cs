

using System;

namespace Prism.Modularity
{
    /// <summary>
    /// Indicates that the class should be considered a named module using the
    /// provided module name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public string ModuleName { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the module should be loaded OnDemand.
        /// </summary>
        /// When <see langword="false"/> (default value), it indicates the module should be loaded as soon as it's dependencies are satisfied.
        /// Otherwise you should explicitily load this module via the <see cref="ModuleManager"/>.
        public bool OnDemand { get; set; }
    }
}
