using Microsoft.Maui.Controls;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Prism.Navigation.Xaml;
using Region = Prism.Navigation.Regions.Region;
using RegionManager = Prism.Navigation.Regions.Xaml.RegionManager;

namespace Prism.Maui.Tests.Fixtures.Regions;

internal sealed class DefaultViewTestRegion
{
    public ContentView Host { get; } = new();
    public Mock<IContainerProvider> Scope { get; } = new(MockBehavior.Strict);
    public Mock<IRegionNavigationRegistry> NavigationRegistry { get; } = new(MockBehavior.Strict);
    public RegionViewRegistry Discovery { get; }
    public Region Region { get; }
    public AutoPopulateRegionBehavior Behavior { get; }

    public DefaultViewTestRegion(object defaultView, RegionViewRegistry discovery = null, bool namedBeforeAttach = true)
    {
        Discovery = discovery ?? new RegionViewRegistry();
        Scope.Setup(c => c.Resolve(typeof(IRegionNavigationRegistry))).Returns(NavigationRegistry.Object);
        NavigationRegistry.SetupGet(r => r.Registrations).Returns(new[]
        {
            new ViewRegistration { Type = ViewType.Region, View = typeof(DefaultRegionView), Name = AutoPopulateDefaultViewFixture.DefaultViewName }
        });
        NavigationRegistry.Setup(r => r.CreateView(Scope.Object, AutoPopulateDefaultViewFixture.DefaultViewName)).Returns(() => new DefaultRegionView());
        Host.SetContainerProvider(Scope.Object);
        RegionManager.SetDefaultView(Host, defaultView);
        Region = new Region(Mock.Of<IRegionNavigationService>()) { TargetElement = Host };
        if (namedBeforeAttach)
            Region.Name = AutoPopulateDefaultViewFixture.RegionName;
        Behavior = new AutoPopulateRegionBehavior(Discovery) { Region = Region, HostControl = Host };
    }
}
