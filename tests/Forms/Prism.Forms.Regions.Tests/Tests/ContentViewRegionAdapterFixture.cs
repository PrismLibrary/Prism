using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Forms.Regions.Mocks;
using Prism.Regions;
using Prism.Regions.Adapters;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class ContentControlRegionAdapterFixture
    {
        [Fact]
        public void AdapterAssociatesSelectorWithRegionActiveViews()
        {
            var control = new ContentView();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");
            Assert.NotNull(region);

            Assert.Null(control.Content);
            region.MockActiveViews.Items.Add(new StackLayout());

            Assert.NotNull(control.Content);
            Assert.Same(control.Content, region.ActiveViews.ElementAt(0));

            region.MockActiveViews.Items.Add(new Grid());
            Assert.Same(control.Content, region.ActiveViews.ElementAt(0));

            region.MockActiveViews.Items.RemoveAt(0);
            Assert.Same(control.Content, region.ActiveViews.ElementAt(0));

            region.MockActiveViews.Items.RemoveAt(0);
            Assert.Null(control.Content);
        }


        [Fact]
        public void ControlWithExistingContentThrows()
        {
            var control = new ContentView() { Content = new StackLayout() };

            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            IRegion region = null;
            var ex = Record.Exception(() => region = (MockPresentationRegion)adapter.Initialize(control, "Region1"));
            Assert.NotNull(ex);
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Contains("ContentView's Content property is not empty.", ex.Message);
        }

        [Fact]
        public void ControlWithExistingBindingOnContentWithNullValueThrows()
        {
            var control = new ContentView();
            var binding = new Binding("ObjectContents")
            {
                Source = new SimpleModel() { ObjectContents = null }
            };
            control.SetBinding(ContentView.ContentProperty, binding);

            IRegionAdapter adapter = new TestableContentControlRegionAdapter();
            IRegion region = null;
            var ex = Record.Exception(() => region = (MockPresentationRegion)adapter.Initialize(control, "Region1"));
            Assert.IsType<InvalidOperationException>(ex);
            Assert.Contains("ContentView's Content property is not empty.", ex.Message);
        }

        [Fact]
        public void AddedItemShouldBeActivated()
        {
            var control = new ContentView();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView = new StackLayout();
            region.Add(mockView);

            Assert.Single(region.ActiveViews);
            Assert.True(region.ActiveViews.Contains(mockView));
        }

        [Fact]
        public void ShouldNotActivateAdditionalViewsAdded()
        {
            var control = new ContentView();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView = new StackLayout();
            region.Add(mockView);
            region.Add(new Grid());

            Assert.Single(region.ActiveViews);
            Assert.True(region.ActiveViews.Contains(mockView));
        }

        [Fact]
        public void ShouldActivateAddedViewWhenNoneIsActive()
        {
            var control = new ContentView();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView1 = new StackLayout();
            region.Add(mockView1);
            region.Deactivate(mockView1);

            var mockView2 = new Grid();
            region.Add(mockView2);

            Assert.Single(region.ActiveViews);
            Assert.True(region.ActiveViews.Contains(mockView2));
        }

        [Fact]
        public void CanRemoveViewWhenNoneActive()
        {
            var control = new ContentView();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView1 = new StackLayout();
            region.Add(mockView1);
            region.Deactivate(mockView1);
            region.Remove(mockView1);
            Assert.Empty(region.ActiveViews);
        }

        class SimpleModel
        {
            public object ObjectContents { get; set; }
        }

        private class TestableContentControlRegionAdapter : ContentViewRegionAdapter
        {
            public TestableContentControlRegionAdapter() : base(null, null)
            {
            }

            private MockPresentationRegion region = new MockPresentationRegion();

            protected override IRegion CreateRegion()
            {
                return region;
            }
        }
    }
}
