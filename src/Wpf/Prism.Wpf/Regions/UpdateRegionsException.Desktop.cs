

using System;
using System.Runtime.Serialization;

namespace Prism.Regions
{
    /// <summary>
    /// Represents errors that occured during the regions' update.
    /// </summary>
    [Serializable]
    public partial class UpdateRegionsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRegionsException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected UpdateRegionsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
