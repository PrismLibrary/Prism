using Prism.Navigation.Regions;
using Xunit;

namespace Prism.Uno.WinUI.Tests;

public class NavigationViewRegionAdapterFixture
{
    [Fact]
    public void AdapterIsSealedAndTargetsNavigationView()
    {
        Assert.True(typeof(NavigationViewRegionAdapter).IsSealed);
        Assert.Equal(typeof(RegionAdapterBase<NavigationView>), typeof(NavigationViewRegionAdapter).BaseType);
    }

    [Fact]
    public void AdapterProvidesExpectedRegionFactoryMethod()
    {
        var method = typeof(NavigationViewRegionAdapter).GetMethod(
            "CreateRegion",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(method);
        Assert.Equal(typeof(IRegion), method!.ReturnType);
    }

    [Fact]
    public void AdapterExposesAdaptOverride()
    {
        var method = typeof(NavigationViewRegionAdapter).GetMethod(
            "Adapt",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(method);
        Assert.Equal(typeof(void), method!.ReturnType);

        var parameters = method.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(typeof(IRegion), parameters[0].ParameterType);
        Assert.Equal(typeof(NavigationView), parameters[1].ParameterType);
    }

    [Fact]
    public void CreateRegionReturnsSingleActiveRegion()
    {
        var behaviorFactory = new Moq.Mock<IRegionBehaviorFactory>();
        var adapter = new NavigationViewRegionAdapter(behaviorFactory.Object);

        var method = typeof(NavigationViewRegionAdapter).GetMethod(
            "CreateRegion",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var region = method!.Invoke(adapter, null);
        Assert.IsType<SingleActiveRegion>(region);
    }
}
