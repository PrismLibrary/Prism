using System;

namespace Prism.Attributes
{
    /// <summary>
    ///     Depends on attribute.
    /// </summary>
    public class DependsOnPropertyAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the DependsOnAttribute class.
        /// </summary>
        public DependsOnPropertyAttribute(params string[] propertyNames)
        {
            SourceProperties = propertyNames;
        }

        /// <summary>
        ///     Gets the source property.
        /// </summary>
        /// <value>The source property.</value>
        public string[] SourceProperties { get; }
    }
}