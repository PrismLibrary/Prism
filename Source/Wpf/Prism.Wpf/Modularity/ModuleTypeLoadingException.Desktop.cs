

using System;
using System.Runtime.Serialization;

namespace Prism.Modularity
{
    [Serializable]
    public partial class ModuleTypeLoadingException
    {
        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ModuleTypeLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
