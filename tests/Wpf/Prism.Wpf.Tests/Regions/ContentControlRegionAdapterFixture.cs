

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Xunit;
using Prism.Regions;
using Prism.Wpf.Tests.Mocks;

namespace Prism.Wpf.Tests.Regions
{
    
    public class ContentControlRegionAdapterFixture
    {
        [StaFact]
        public void AdapterAssociatesSelectorWithRegionActiveViews()
        {
            var control = new ContentControl();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");
            Assert.NotNull(region);

            Assert.Null(control.Content);
            region.MockActiveViews.Items.Add(new object());

            Assert.NotNull(control.Content);
            Assert.Same(control.Content, region.ActiveViews.ElementAt(0));

            region.MockActiveViews.Items.Add(new object());
            Assert.Same(control.Content, region.ActiveViews.ElementAt(0));

            region.MockActiveViews.Items.RemoveAt(0);
            Assert.Same(control.Content, region.ActiveViews.ElementAt(0));

            region.MockActiveViews.Items.RemoveAt(0);
            Assert.Null(control.Content);
        }


        [StaFact]
        public void ControlWithExistingContentThrows()
        {
            var control = new ContentControl() { Content = new object() };

            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            try
            {
                var region = (MockPresentationRegion)adapter.Initialize(control, "Region1");
                //Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("ContentControl's Content property is not empty.", ex.Message);
            }
        }

        [StaFact]
        public void ControlWithExistingBindingOnContentWithNullValueThrows()
        {
            var control = new ContentControl();
            Binding binding = new Binding("ObjectContents");
            binding.Source = new SimpleModel() { ObjectContents = null };
            control.SetBinding(ContentControl.ContentProperty, binding);

            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            try
            {
                var region = (MockPresentationRegion)adapter.Initialize(control, "Region1");
                //Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("ContentControl's Content property is not empty.", ex.Message);
            }
        }

        [StaFact]
        public void AddedItemShouldBeActivated()
        {
            var control = new ContentControl();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView = new object();
            region.Add(mockView);

            Assert.Single(region.ActiveViews);
            Assert.True(region.ActiveViews.Contains(mockView));
        }

        [StaFact]
        public void ShouldNotActivateAdditionalViewsAdded()
        {
            var control = new ContentControl();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView = new object();
            region.Add(mockView);
            region.Add(new object());

            Assert.Single(region.ActiveViews);
            Assert.True(region.ActiveViews.Contains(mockView));
        }

        [StaFact]
        public void ShouldActivateAddedViewWhenNoneIsActive()
        {
            var control = new ContentControl();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView1 = new object();
            region.Add(mockView1);
            region.Deactivate(mockView1);

            var mockView2 = new object();
            region.Add(mockView2);

            Assert.Single(region.ActiveViews);
            Assert.True(region.ActiveViews.Contains(mockView2));
        }

        [StaFact]
        public void CanRemoveViewWhenNoneActive()
        {
            var control = new ContentControl();
            IRegionAdapter adapter = new TestableContentControlRegionAdapter();

            MockPresentationRegion region = (MockPresentationRegion)adapter.Initialize(control, "Region1");

            var mockView1 = new object();
            region.Add(mockView1);
            region.Deactivate(mockView1);
            region.Remove(mockView1);
            Assert.Empty(region.ActiveViews);
        }

        class SimpleModel
        {
            public Object ObjectContents { get; set; }
        }

        private class TestableContentControlRegionAdapter : ContentControlRegionAdapter
        {
            public TestableContentControlRegionAdapter() : base(null)
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