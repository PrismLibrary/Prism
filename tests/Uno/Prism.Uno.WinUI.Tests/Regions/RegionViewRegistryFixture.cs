using System;
using System.Linq;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Uno.WinUI.Tests.Regions;

public class RegionViewRegistryFixture
{
    private class MockContentObject
    {
    }

    [Fact]
    public void RegisterViewWithRegion_DuplicateTypeRegistration_ReturnsSingleView()
    {
        var containerMock = new Mock<IContainerExtension>();
        ContainerLocator.SetContainerExtension(containerMock.Object);
        containerMock.Setup(c => c.Resolve(typeof(MockContentObject))).Returns(new MockContentObject());
        var registry = new RegionViewRegistry(containerMock.Object);

        registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));
        registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));

        var result = registry.GetContents("MyRegion");

        Assert.Single(result);
    }

    [Fact]
    public void RegisterViewWithRegion_DuplicateTargetNameRegistration_ReturnsSingleView()
    {
        var containerMock = new Mock<IContainerExtension>();
        ContainerLocator.SetContainerExtension(containerMock.Object);
        var content = new MockContentObject();
        containerMock.Setup(c => c.Resolve(typeof(object), "MyView")).Returns(content);
        var registry = new RegionViewRegistry(containerMock.Object);

        registry.RegisterViewWithRegion("MyRegion", "MyView");
        registry.RegisterViewWithRegion("MyRegion", "MyView");

        var result = registry.GetContents("MyRegion");

        Assert.Single(result);
    }
}
