using System;
using System.Runtime.Serialization;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Exception that's thrown when something goes wrong while Registering a View with a region name in the <see cref="IRegionViewRegistry"/> class.
    /// </summary>
    [Serializable]
    public partial class ViewRegistrationException : Exception
    {
        // TODO: Find updated links as these are dead...
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRegistrationException"/> class.
        /// </summary>
        public ViewRegistrationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ViewRegistrationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public ViewRegistrationException(string message, Exception inner) : base(message, inner)
        {
        }

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
