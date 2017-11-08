using Unity.Builder;
using Unity.Extension;

namespace Prism.Unity.Extensions
{
    public class DependencyServiceExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.Add(new DependencyServiceStrategy(Container), UnityBuildStage.PreCreation);
        }
    }
}
