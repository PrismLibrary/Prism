

using System;
using System.Runtime.Serialization;

namespace Prism.Modularity
{
    /// <summary>
    /// Exception thrown when a requested <see cref="ModuleInfo"/> is not found.
    /// </summary>
    [Serializable]
    public partial class ModuleNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNotFoundException"/> class
        /// with the serialization data.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination.</param>
        protected ModuleNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
