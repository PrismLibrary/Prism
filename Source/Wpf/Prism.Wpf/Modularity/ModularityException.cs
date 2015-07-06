

using System;

namespace Prism.Modularity
{
    /// <summary>
    /// Base class for exceptions that are thrown because of a problem with modules. 
    /// </summary>
    public partial class ModularityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModularityException"/> class.
        /// </summary>
        public ModularityException()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModularityException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ModularityException(string message)
            : this(null, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModularityException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ModularityException(string message, Exception innerException)
            : this(null, message, innerException)
        {
        }

        /// <summary>
        /// Initializes the exception with a particular module and error message.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ModularityException(string moduleName, string message)
            : this(moduleName, message, null)
        {
        }

        /// <summary>
        /// Initializes the exception with a particular module, error message and inner exception that happened.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a <see langword="null"/> reference if no inner exception is specified.</param>
        public ModularityException(string moduleName, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ModuleName = moduleName;
        }

        /// <summary>
        /// Gets or sets the name of the module that this exception refers to.
        /// </summary>
        /// <value>The name of the module.</value>
        public string ModuleName { get; set; }
    }
}
