using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.AppModel
{
    /// <summary>
    /// Implementation of <see cref="IApplicationStore"/>
    /// </summary>
    public class ApplicationStore : IApplicationStore
    {
        /// <summary>
        /// Getter for the current ApplicationStore properties
        /// <see cref="IApplicationStore.Properties"/>
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get { return Application.Current.Properties; }
        }

        /// <summary>
        /// Asynchronously persists the Application.Properties dictionary for the application object.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation
        /// <see cref="IApplicationStore.SavePropertiesAsync"/></returns>
        public Task SavePropertiesAsync() =>
            Application.Current.SavePropertiesAsync();
    }
}
