using System;

namespace Prism.AppModel
{
    /// <summary>
    /// Provides additional context for a property that will be initialized with <see cref="IAutoInitialize"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class AutoInitializeAttribute : Attribute
    {
        /// <summary>
        /// Provides a custom name for <see cref="IAutoInitialize"/> to use
        /// </summary>
        /// <param name="name">The custom name to use</param>
        public AutoInitializeAttribute(string name) : this(name, false) { }

        /// <summary>
        /// Marks a property as required if <c>true</c>
        /// </summary>
        /// <param name="isRequired">Makes property required if <c>true</c></param>
        public AutoInitializeAttribute(bool isRequired) : this(null, isRequired) { }

        /// <summary>
        /// Provides a custom name for <see cref="IAutoInitialize"/> to use and makes it required
        /// </summary>
        /// <param name="name">The custom name to use</param>
        /// <param name="isRequired">Makes property required if <c>true</c></param>
        public AutoInitializeAttribute(string name, bool isRequired)
        {
            Name = name;
            IsRequired = isRequired;
        }

        /// <summary>
        /// Gets the IsRequired value
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        /// Gets the Custom Property Name
        /// </summary>
        public string Name { get; }
    }
}
