using System.Collections.Generic;
using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefOnlyWithMefOnlyDependenciesModel))]
    public class MefOnlyWithMefOnlyDependenciesModel : IMefOnlyWithMefOnlyDependenciesModel
    {
        [Import]
        public IMefOnly1Model MefOnly1Model { get; set; }

        [ImportMany]
        public IEnumerable<IMefOnlyModels> MefOnlyModels { get; set; }
    }
}
