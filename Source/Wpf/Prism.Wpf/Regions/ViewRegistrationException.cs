

using System;

namespace Prism.Regions
{
    /// <summary>
    /// Exception that's thrown when something goes wrong while Registering a View with a region name in the <see cref="RegionViewRegistry"/> class. 
    /// </summary>
    public partial class ViewRegistrationException : Exception
    {
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

       
    }
}