using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prism.AppModel
{
    /// <summary>
    /// Interface for an Application Store to save/load properties of Application
    /// </summary>
    public interface IApplicationStore
    {
        /// <summary>
        /// Getter for properties
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Asynchronously persists the Application.Properties dictionary for the application object.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation
        /// <see cref="IApplicationStore.SavePropertiesAsync"/></returns>
        Task SavePropertiesAsync();
    }
}
