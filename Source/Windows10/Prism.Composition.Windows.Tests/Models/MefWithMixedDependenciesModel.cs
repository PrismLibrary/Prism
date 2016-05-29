using System.Collections.Generic;
using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefWithMixedDependenciesModel))]
    public class MefWithMixedDependenciesModel : IMefWithMixedDependenciesModel
    {
        [ImportMany]
        public IEnumerable<IMixedModels> MixedModels { get; set; }
    }
}
