using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prism.AppModel
{
    public interface IApplicationStore
    {
        IDictionary<string, object> Properties { get; }

        Task SavePropertiesAsync();
    }
}
