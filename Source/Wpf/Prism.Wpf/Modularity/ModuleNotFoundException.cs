

using System;

namespace Prism.Modularity
{
    /// <summary>
    /// Exception thrown when a requested <see cref="InitializationMode.OnDemand"/> <see cref="IModule"/> was not found.
    /// </summary>
    public partial class ModuleNotFoundException : ModularityException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNotFoundException" /> class.
        /// </summary>
        public ModuleNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNotFoundException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error. 
        /// </param>
        public ModuleNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNotFoundException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error. 
        /// </param>
        /// <param name="innerException">The inner exception</param>
        public ModuleNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ModuleNotFoundException(string moduleName, string message)
            : base(moduleName, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ModuleNotFoundException(string moduleName, string message, Exception innerException)
            : base(moduleName, message, innerException)
        {
        }
    }
}
