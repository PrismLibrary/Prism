using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefWithSpecificUnityDependencyModel))]
    public class MefWithSpecificUnityDependencyModel : IMefWithSpecificUnityDependencyModel
    {
        [Import("Model2")]
        public IUnityOnlyModels UnityOnlyModel { get; set; }
    }
}
