

using Xunit;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    
    public class BindRegionContextToDependencyObjectBehaviorFixture
    {
        [StaFact]
        public void ShouldSetRegionContextOnAddedView()
        {
            var behavior = new BindRegionContextToDependencyObjectBehavior();
            var region = new MockPresentationRegion();
            behavior.Region = region;
            region.Context = "MyContext";
            var view = new MockDependencyObject();

            behavior.Attach();
            region.Add(view);

            var context = RegionContext.GetObservableContext(view);
            Assert.NotNull(context.Value);
            Assert.Equal("MyContext", context.Value);
        }

        [StaFact]
        public void ShouldSetRegionContextOnAlreadyAddedViews()
        {
            var behavior = new BindRegionContextToDependencyObjectBehavior();
            var region = new MockPresentationRegion();
            var view = new MockDependencyObject();
            region.Add(view);
            behavior.Region = region;
            region.Context = "MyContext";

            behavior.Attach();

            var context = RegionContext.GetObservableContext(view);
            Assert.NotNull(context.Value);
            Assert.Equal("MyContext", context.Value);
        }

        [StaFact]
        public void ShouldRemoveContextToViewRemovedFromRegion()
        {
            var behavior = new BindRegionContextToDependencyObjectBehavior();
            var region = new MockPresentationRegion();
            var view = new MockDependencyObject();
            region.Add(view);
            behavior.Region = region;
            region.Context = "MyContext";
            behavior.Attach();

            region.Remove(view);

            var context = RegionContext.GetObservableContext(view);
            Assert.Null(context.Value);
        }

        [StaFact]
        public void ShouldSetRegionContextOnContextChange()
        {
            var behavior = new BindRegionContextToDependencyObjectBehavior();
            var region = new MockPresentationRegion();
            var view = new MockDependencyObject();
            region.Add(view);
            behavior.Region = region;
            region.Context = "MyContext";
            behavior.Attach();
            Assert.Equal("MyContext", RegionContext.GetObservableContext(view).Value);

            region.Context = "MyNewContext";
            region.OnPropertyChange("Context");

            Assert.Equal("MyNewContext", RegionContext.GetObservableContext(view).Value);
        }

        [StaFact]
        public void WhenAViewIsRemovedFromARegion_ThenRegionContextIsNotClearedInRegion()
        {
            var behavior = new BindRegionContextToDependencyObjectBehavior();
            var region = new MockPresentationRegion();

            behavior.Region = region;
            behavior.Attach();

            var myView = new MockFrameworkElement();
            
            region.Add(myView);
            region.Context = "new context";

            region.Remove(myView);

            Assert.NotNull(region.Context);
        }
    }
}