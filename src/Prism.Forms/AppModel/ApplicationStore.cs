using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.AppModel
{
    public class ApplicationStore : IApplicationStore
    {
        public IDictionary<string, object> Properties
        {
            get { return Application.Current.Properties; }
        }

        public Task SavePropertiesAsync() =>
            Application.Current.SavePropertiesAsync();
    }
}
