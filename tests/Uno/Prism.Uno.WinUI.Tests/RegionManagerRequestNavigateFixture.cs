using Moq;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class RegionManagerRequestNavigateFixture
{
    private const string Region = "Region";
    private const string NonExistentRegion = "NonExistentRegion";
    private const string Source = "Source";
    private static readonly Uri SourceUri = new("Source", UriKind.RelativeOrAbsolute);
    private static readonly NavigationParameters Parameters = new();

    [Fact]
    public void ThrowsWhenNavigationCallbackIsNull()
    {
        var regionManager = new RegionManager();

        Assert.Throws<ArgumentNullException>(() => regionManager.RequestNavigate(Region, Source, null!, Parameters));
        Assert.Throws<ArgumentNullException>(() => regionManager.RequestNavigate(Region, Source, navigationCallback: null!));
        Assert.Throws<ArgumentNullException>(() => regionManager.RequestNavigate(Region, SourceUri, null!));
        Assert.Throws<ArgumentNullException>(() => regionManager.RequestNavigate(Region, SourceUri, null!, Parameters));
    }

    [Fact]
    public void ReturnsFailedResultWhenRegionDoesNotExist()
    {
        var regionManager = new RegionManager();
        NavigationResult? result = null;

        regionManager.RequestNavigate(NonExistentRegion, Source, r => result = r, Parameters);
        Assert.NotNull(result);
        Assert.False(result!.Success);

        result = null;
        regionManager.RequestNavigate(NonExistentRegion, Source, r => result = r);
        Assert.NotNull(result);
        Assert.False(result!.Success);

        result = null;
        regionManager.RequestNavigate(NonExistentRegion, SourceUri, r => result = r, Parameters);

        Assert.NotNull(result);
        Assert.False(result!.Success);
    }

    [Fact]
    public void DelegatesCallToMatchingRegion()
    {
        var region = new Mock<IRegion>();
        region.SetupGet(x => x.Name).Returns(Region);

        var regionManager = new RegionManager();
        regionManager.Regions.Add(region.Object);

        regionManager.RequestNavigate(Region, SourceUri, _ => { }, Parameters);

        region.Verify(x => x.RequestNavigate(SourceUri, It.IsAny<Action<NavigationResult>>(), Parameters), Times.Once);
    }

    [Fact]
    public void DelegatesCallToRegionForSourceStringOverloads()
    {
        var region = new Mock<IRegion>();
        region.SetupGet(x => x.Name).Returns(Region);
        var callback = new Action<NavigationResult>(_ => { });

        var regionManager = new RegionManager();
        regionManager.Regions.Add(region.Object);

        regionManager.RequestNavigate(Region, Source);
        regionManager.RequestNavigate(Region, Source, Parameters);
        regionManager.RequestNavigate(Region, Source, callback);
        regionManager.RequestNavigate(Region, Source, callback, Parameters);

        region.Verify(x => x.RequestNavigate(SourceUri, It.IsAny<Action<NavigationResult>>(), It.IsAny<INavigationParameters>()), Times.AtLeastOnce);
        region.Verify(x => x.RequestNavigate(SourceUri, callback, It.IsAny<INavigationParameters>()), Times.AtLeastOnce);
        region.Verify(x => x.RequestNavigate(SourceUri, callback, Parameters), Times.AtLeastOnce);
    }

    [Fact]
    public void DelegatesCallToRegionForUriOverloads()
    {
        var region = new Mock<IRegion>();
        region.SetupGet(x => x.Name).Returns(Region);
        var callback = new Action<NavigationResult>(_ => { });

        var regionManager = new RegionManager();
        regionManager.Regions.Add(region.Object);

        regionManager.RequestNavigate(Region, SourceUri);
        regionManager.RequestNavigate(Region, SourceUri, Parameters);
        regionManager.RequestNavigate(Region, SourceUri, callback);
        regionManager.RequestNavigate(Region, SourceUri, callback, Parameters);

        region.Verify(x => x.RequestNavigate(SourceUri, It.IsAny<Action<NavigationResult>>(), It.IsAny<INavigationParameters>()), Times.AtLeastOnce);
        region.Verify(x => x.RequestNavigate(SourceUri, callback, It.IsAny<INavigationParameters>()), Times.AtLeastOnce);
        region.Verify(x => x.RequestNavigate(SourceUri, callback, Parameters), Times.AtLeastOnce);
    }
}
