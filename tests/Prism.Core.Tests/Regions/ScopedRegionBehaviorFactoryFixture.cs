using System;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Core.Tests.Regions;

public class ScopedRegionBehaviorFactoryFixture
{
    [Fact]
    public void ExplicitProviderResolvesBehaviorWithoutUsingRoot()
    {
        var root = new Mock<IContainerExtension>(MockBehavior.Strict);
        var owner = new Mock<IContainerProvider>(MockBehavior.Strict);
        var behavior = new Mock<IRegionBehavior>().Object;
        owner.Setup(p => p.Resolve(typeof(TestBehavior),
                It.Is<(Type, object)[]>(args => args.Length == 1 &&
                    args[0].Item1 == typeof(IContainerProvider) && ReferenceEquals(args[0].Item2, owner.Object))))
            .Returns(behavior);
        var factory = new RegionBehaviorFactory(root.Object);
        factory.AddIfMissing("owned", typeof(TestBehavior));

        Assert.Same(behavior, factory.CreateFromKey("owned", owner.Object));
        owner.VerifyAll();
        root.VerifyNoOtherCalls();
    }

    [Fact]
    public void ExplicitProviderRequiresRegisteredKey()
    {
        var factory = new RegionBehaviorFactory(null);
        var owner = new Mock<IContainerProvider>(MockBehavior.Strict);
        Assert.Throws<ArgumentException>(() => factory.CreateFromKey("missing", owner.Object));
        owner.VerifyNoOtherCalls();
    }

    [Fact]
    public void ExplicitProviderCannotBeNull()
    {
        var factory = new RegionBehaviorFactory(null);
        Assert.Throws<ArgumentNullException>(() => factory.CreateFromKey("key", null));
    }

    private sealed class TestBehavior : IRegionBehavior
    {
        public IRegion Region { get; set; }
        public void Attach() { }
    }
}
