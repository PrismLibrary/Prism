using System;

namespace Prism.Modularity
{
    /// <summary>
    /// Represents the exception that is thrown when there is a circular dependency
    /// between modules during the module loading process.
    /// </summary>
    public partial class CyclicDependencyFoundException : ModularityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicDependencyFoundException"/> class.
        /// </summary>
        public CyclicDependencyFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicDependencyFoundException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CyclicDependencyFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicDependencyFoundException"/> class
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public CyclicDependencyFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes the exception with a particular module, error message and inner exception that happened.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a <see langword="null"/> reference if no inner exception is specified.</param>
        public CyclicDependencyFoundException(string moduleName, string message, Exception innerException)
            : base(moduleName, message, innerException)
        {
        }
    }
}