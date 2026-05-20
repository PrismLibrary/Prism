using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Xunit;

namespace Prism.Uno.WinUI.Tests.Regions.Behaviors;

public class AutoPopulateRegionBehaviorFixture
{
    private class MockContentObject
    {
    }

    [Fact]
    public void WhenSameViewTypeRegisteredTwice_RegionContainsSingleView()
    {
        var containerMock = new Mock<IContainerExtension>();
        ContainerLocator.SetContainerExtension(containerMock.Object);
        containerMock.Setup(c => c.Resolve(typeof(MockContentObject))).Returns(new MockContentObject());
        var registry = new RegionViewRegistry(containerMock.Object);
        registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));
        registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));

        var region = new Region { Name = "MyRegion" };
        var behavior = new AutoPopulateRegionBehavior(registry)
        {
            Region = region
        };

        behavior.Attach();

        Assert.Single(region.Views);
    }

    [Fact]
    public void ShouldAddDefaultViewByNameWhenRegionIsPopulated()
    {
        var containerMock = new Mock<IContainerExtension>();
        ContainerLocator.SetContainerExtension(containerMock.Object);
        var content = new object();
        containerMock.Setup(c => c.Resolve(typeof(object), "MyView")).Returns(content);
        var registry = new RegionViewRegistry(containerMock.Object);
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "MyView");

        var region = new Region { Name = "MyRegion" };
        var behavior = new AutoPopulateRegionBehavior(registry)
        {
            Region = region,
            HostControl = host
        };

        behavior.Attach();

        Assert.Single(region.Views);
        Assert.Same(content, region.Views.ElementAt(0));
    }

    [Fact]
    public void WhenDefaultViewMatchesRegisteredView_RegionContainsSingleView()
    {
        var containerMock = new Mock<IContainerExtension>();
        ContainerLocator.SetContainerExtension(containerMock.Object);
        var content = new object();
        containerMock.Setup(c => c.Resolve(typeof(object), "MyView")).Returns(content);
        var registry = new RegionViewRegistry(containerMock.Object);
        registry.RegisterViewWithRegion("MyRegion", "MyView");
        var host = new ContentControl();
        RegionManager.SetDefaultView(host, "MyView");

        var region = new Region { Name = "MyRegion" };
        var behavior = new AutoPopulateRegionBehavior(registry)
        {
            Region = region,
            HostControl = host
        };

        behavior.Attach();

        Assert.Single(region.Views);
    }
}
