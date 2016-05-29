namespace Prism.Composition.Windows.Tests.Models
{
    public interface IMefWithSpecificUnityDependencyModel
    {
        IUnityOnlyModels UnityOnlyModel { get; set; }
    }
}