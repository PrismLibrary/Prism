

using System;
using System.Runtime.Serialization;

namespace Prism.Modularity
{
    [Serializable]
    public partial class DuplicateModuleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateModuleException"/> class
        /// with the serialization data.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination.</param>
        protected DuplicateModuleException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
