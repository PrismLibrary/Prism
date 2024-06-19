using Prism.DryIoc.Maui.Tests.Mocks.Regions.Behaviors;
using Prism.Navigation.Regions.Behaviors;

namespace Prism.DryIoc.Maui.Tests.Fixtures.Regions;
public class RegionBehaviorFixture : TestBase
{
    public RegionBehaviorFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public void ExistingBehavior_IsReplaced_WithCustomBehavior()
    {
        var mauiApp = CreateBuilder(prism => prism
            .ConfigureRegionBehaviors(behaviors =>
            {
                behaviors.AddOrReplace<RegionBehaviorBMock>(RegionBehaviorAMock.BehaviorKey);
            }))
            .Build();

        var regionBehaviorFactory = mauiApp.Services.GetRequiredService<IRegionBehaviorFactory>();

        Assert.NotEmpty(regionBehaviorFactory);
        Assert.Contains(regionBehaviorFactory, x => x == RegionBehaviorAMock.BehaviorKey);
        Assert.DoesNotContain(regionBehaviorFactory, x => x == RegionBehaviorBMock.BehaviorKey);
        Assert.IsType<RegionBehaviorBMock>(regionBehaviorFactory.CreateFromKey(RegionBehaviorAMock.BehaviorKey));
    }

    [Fact]
    public void MissingBehavior_IsAdded()
    {
        var mauiApp = CreateBuilder(prism => prism
            .ConfigureRegionBehaviors(behaviors =>
            {
                behaviors.AddOrReplace<RegionBehaviorAMock>(RegionBehaviorAMock.BehaviorKey);
            }))
            .Build();

        var regionBehaviorFactory = mauiApp.Services.GetRequiredService<IRegionBehaviorFactory>();

        Assert.NotEmpty(regionBehaviorFactory);
        Assert.Contains(regionBehaviorFactory, x => x == RegionBehaviorAMock.BehaviorKey);
        Assert.IsType<RegionBehaviorAMock>(regionBehaviorFactory.CreateFromKey(RegionBehaviorAMock.BehaviorKey));
    }
}
