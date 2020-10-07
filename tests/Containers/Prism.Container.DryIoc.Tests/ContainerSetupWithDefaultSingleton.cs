using DryIoc;
using Prism.DryIoc;
using Prism.Ioc.Tests;

namespace Prism.Ioc.DryIoc.Tests
{
    public class ContainerSetupWithDefaultSingleton : ContainerSetup
    {
        public static Rules RulesWithDefaultSingleton => DryIocContainerExtension.DefaultRules.WithDefaultReuse(Reuse.Singleton);

        protected override IContainerExtension CreateContainerInternal() => new DryIocContainerExtension(new Container(RulesWithDefaultSingleton));
    }
}
