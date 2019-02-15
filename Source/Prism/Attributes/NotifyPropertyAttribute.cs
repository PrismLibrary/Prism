using System;

namespace Prism.Attributes
{
    /// <summary>
    ///     Depends on attribute.
    /// </summary>
    public class NotifyPropertyAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the NotifyAttribute class.
        /// </summary>
        public NotifyPropertyAttribute(params string[] propertyNames)
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