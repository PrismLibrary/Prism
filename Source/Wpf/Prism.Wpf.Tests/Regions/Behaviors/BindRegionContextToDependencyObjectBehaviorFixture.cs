

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions.Behaviors
{
    [TestClass]
    public class BindRegionContextToDependencyObjectBehaviorFixture
    {
        [TestMethod]
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
            Assert.IsNotNull(context.Value);
            Assert.AreEqual("MyContext", context.Value);
        }

        [TestMethod]
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
            Assert.IsNotNull(context.Value);
            Assert.AreEqual("MyContext", context.Value);
        }

        [TestMethod]
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
            Assert.IsNull(context.Value);
        }

        [TestMethod]
        public void ShouldSetRegionContextOnContextChange()
        {
            var behavior = new BindRegionContextToDependencyObjectBehavior();
            var region = new MockPresentationRegion();
            var view = new MockDependencyObject();
            region.Add(view);
            behavior.Region = region;
            region.Context = "MyContext";
            behavior.Attach();
            Assert.AreEqual("MyContext", RegionContext.GetObservableContext(view).Value);

            region.Context = "MyNewContext";
            region.OnPropertyChange("Context");

            Assert.AreEqual("MyNewContext", RegionContext.GetObservableContext(view).Value);
        }

        [TestMethod]
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

            Assert.IsNotNull(region.Context);
        }
    }
}