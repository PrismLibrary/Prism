

using System;
using System.Runtime.Serialization;

namespace Prism.Regions
{
    /// <summary>
    /// Exception that's thrown when something goes wrong while Registering a View with a region name in the <see cref="RegionViewRegistry"/> class. 
    /// </summary>
    [Serializable]
    public partial class ViewRegistrationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRegistrationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ViewRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}