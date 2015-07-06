

using System;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Represents errors that occured during region creation.
    /// </summary>
    public partial class RegionCreationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionCreationException"/>
        /// </summary>
        public RegionCreationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionCreationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RegionCreationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionCreationException"/> class with a specified error message and a reference 
        /// to the inner exception that is the cause of this exception. 
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference 
        /// (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RegionCreationException(string message, Exception inner) 
            : base(message, inner)
        {
        }
    }
}
