using System.Collections.Generic;

namespace Prism.Composition.Windows.Tests.Models
{
    public interface IMefWithMixedDependenciesModel
    {
        IEnumerable<IMixedModels> MixedModels { get; set; }
    }
}