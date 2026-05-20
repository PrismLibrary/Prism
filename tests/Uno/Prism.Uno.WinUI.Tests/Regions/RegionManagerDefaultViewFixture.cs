using Microsoft.UI.Xaml.Controls;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Xunit;

namespace Prism.Uno.WinUI.Tests.Regions;

public class RegionManagerDefaultViewFixture
{
    private class MockContentObject
    {
    }

    [Fact]
    public void SettingDefaultViewType_RegistersViewWithRegion()
    {
        var mockRegistry = new Mock<IRegionViewRegistry>();
        var containerMock = new Mock<IContainerExtension>();
        containerMock.Setup(c => c.Resolve(typeof(IRegionViewRegistry))).Returns(mockRegistry.Object);
        containerMock.Setup(c => c.Resolve(typeof(DelayedRegionCreationBehavior)))
            .Returns(new DelayedRegionCreationBehavior(new RegionAdapterMappings()));
        ContainerLocator.SetContainerExtension(containerMock.Object);

        var host = new ContentControl();
        RegionManager.SetRegionName(host, "MyRegion");
        RegionManager.SetDefaultView(host, typeof(MockContentObject));

        mockRegistry.Verify(r => r.RegisterViewWithRegion("MyRegion", typeof(MockContentObject)), Times.Once);
    }

    [Fact]
    public void SettingDefaultViewTwice_RegistersOnceDueToDuplicateGuard()
    {
        var containerMock = new Mock<IContainerExtension>();
        ContainerLocator.SetContainerExtension(containerMock.Object);
        containerMock.Setup(c => c.Resolve(typeof(MockContentObject))).Returns(new MockContentObject());
        containerMock.Setup(c => c.Resolve(typeof(DelayedRegionCreationBehavior)))
            .Returns(new DelayedRegionCreationBehavior(new RegionAdapterMappings()));
        var registry = new RegionViewRegistry(containerMock.Object);
        containerMock.Setup(c => c.Resolve(typeof(IRegionViewRegistry))).Returns(registry);

        var host = new ContentControl();
        RegionManager.SetRegionName(host, "MyRegion");
        RegionManager.SetDefaultView(host, typeof(MockContentObject));
        RegionManager.SetDefaultView(host, typeof(MockContentObject));

        var region = new Region { Name = "MyRegion" };
        var behavior = new AutoPopulateRegionBehavior(registry) { Region = region };
        behavior.Attach();

        Assert.Single(region.Views);
    }
}
