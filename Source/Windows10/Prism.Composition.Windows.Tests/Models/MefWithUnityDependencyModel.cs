using System.Composition;

namespace Prism.Composition.Windows.Tests.Models
{
    [Export(typeof(IMefWithUnityDependencyModel))]
    public class MefWithUnityDependencyModel : IMefWithUnityDependencyModel
    {
        [Import]
        public IUnityOnly1Model UnityOnly1Model { get; set; }
    }
}
