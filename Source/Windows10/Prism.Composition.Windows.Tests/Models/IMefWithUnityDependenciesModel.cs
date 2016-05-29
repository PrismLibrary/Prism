using System.Collections.Generic;

namespace Prism.Composition.Windows.Tests.Models
{
    public interface IMefWithUnityDependenciesModel
    {
        IEnumerable<IUnityOnlyModels> UnityOnlyModels { get; set; }
    }
}