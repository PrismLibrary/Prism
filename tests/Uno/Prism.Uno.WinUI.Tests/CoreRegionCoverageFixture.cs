using System.Collections.Specialized;
using Moq;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class CoreRegionCoverageFixture
{
    [Fact]
    public void RegionManagerCanAddAndRetrieveRegionByName()
    {
        var regionManager = new RegionManager();
        var region = new Region { Name = "MainRegion" };

        regionManager.Regions.Add(region);

        Assert.Same(region, regionManager.Regions["MainRegion"]);
    }

    [Fact]
    public void RegionManagerContainsRegionWithNameReflectsState()
    {
        var regionManager = new RegionManager();
        Assert.False(regionManager.Regions.ContainsRegionWithName("Target"));

        regionManager.Regions.Add(new Region { Name = "Target" });
        Assert.True(regionManager.Regions.ContainsRegionWithName("Target"));
    }

    [Fact]
    public void RegionManagerRemovingRegionClearsRegionManagerReference()
    {
        var regionManager = new RegionManager();
        var region = new Region { Name = "RemoveMe" };
        regionManager.Regions.Add(region);

        var removed = regionManager.Regions.Remove("RemoveMe");

        Assert.True(removed);
        Assert.Null(region.RegionManager);
    }

    [Fact]
    public void RegionManagerCreateRegionManagerReturnsNewInstance()
    {
        var manager = new RegionManager();
        var child = manager.CreateRegionManager();

        Assert.NotNull(child);
        Assert.NotSame(manager, child);
        Assert.IsType<RegionManager>(child);
    }

    [Fact]
    public void RegionManagerCollectionChangedFiresOnAddAndRemove()
    {
        var manager = new RegionManager();
        NotifyCollectionChangedEventArgs? lastArgs = null;
        manager.Regions.CollectionChanged += (_, e) => lastArgs = e;
        var region = new Region { Name = "Region1" };

        manager.Regions.Add(region);
        Assert.NotNull(lastArgs);
        Assert.Equal(NotifyCollectionChangedAction.Add, lastArgs!.Action);

        manager.Regions.Remove("Region1");
        Assert.NotNull(lastArgs);
        Assert.Equal(NotifyCollectionChangedAction.Remove, lastArgs!.Action);
    }

    [Fact]
    public void RegionGetViewReturnsNullForUnknownName()
    {
        var region = new Region();
        Assert.Null(region.GetView("Unknown"));
    }

    [Fact]
    public void RegionNameCannotBeChangedAfterFirstSet()
    {
        var region = new Region { Name = "Initial" };
        Assert.Throws<InvalidOperationException>(() => region.Name = "Changed");
    }

    [Fact]
    public void RegionContextChangeRaisesPropertyChanged()
    {
        var region = new Region();
        var changed = false;
        region.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(region.Context))
            {
                changed = true;
            }
        };

        region.Context = "CTX";
        Assert.True(changed);
    }

    [Fact]
    public void NavigationContextParsesQueryParametersFromUri()
    {
        var uri = new Uri("target?name=value", UriKind.Relative);
        var navService = new Mock<IRegionNavigationService>();
        navService.SetupGet(x => x.Journal).Returns(Mock.Of<IRegionNavigationJournal>());

        var context = new NavigationContext(navService.Object, uri);

        Assert.Equal(uri, context.Uri);
        Assert.NotNull(context.Parameters);
    }

    [Fact]
    public void NavigationContextWithoutQueryHasEmptyParameters()
    {
        var uri = new Uri("target", UriKind.Relative);
        var navService = new Mock<IRegionNavigationService>();
        navService.SetupGet(x => x.Journal).Returns(Mock.Of<IRegionNavigationJournal>());

        var context = new NavigationContext(navService.Object, uri);

        Assert.Empty(context.Parameters);
    }
}
