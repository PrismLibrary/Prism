using System.Collections.Generic;
using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefWithUnityDependenciesModel))]
    public class MefWithUnityDependenciesModel : IMefWithUnityDependenciesModel
    {
        [ImportMany]
        public IEnumerable<IUnityOnlyModels> UnityOnlyModels { get; set; }
    }
}
