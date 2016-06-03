using System.Collections.Generic;

namespace Prism.Composition.Windows.Tests.Models
{
    public interface IMefOnlyWithMefOnlyDependenciesModel
    {
        IMefOnly1Model MefOnly1Model { get; set; }

        IEnumerable<IMefOnlyModels> MefOnlyModels { get; set; }
    }
}