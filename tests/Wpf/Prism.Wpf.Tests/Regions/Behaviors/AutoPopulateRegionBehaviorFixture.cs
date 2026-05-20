using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Moq;
using Prism.Ioc;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Behaviors;
using Prism.Wpf.Tests.Mocks;
using Xunit;

namespace Prism.Wpf.Tests.Regions.Behaviors
{

    public class AutoPopulateRegionBehaviorFixture
    {
        private class MockContentObject
        {
        }

        [Fact]
        public void ShouldGetViewsFromRegistryOnAttach()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var region = new MockPresentationRegion() { Name = "MyRegion" };
            var viewFactory = new MockRegionContentRegistry();
            var view = new object();
            viewFactory.GetContentsReturnValue.Add(view);
            var behavior = new AutoPopulateRegionBehavior(viewFactory)
            {
                Region = region
            };

            behavior.Attach();

            Assert.Equal("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.Single(region.MockViews.Items);
            Assert.Equal(view, region.MockViews.Items[0]);
        }

        [Fact]
        public void ShouldGetViewsFromRegistryWhenRegisteringItAfterAttach()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var region = new MockPresentationRegion() { Name = "MyRegion" };
            var viewFactory = new MockRegionContentRegistry();
            var behavior = new AutoPopulateRegionBehavior(viewFactory)
            {
                Region = region
            };
            var view = new object();

            behavior.Attach();
            viewFactory.GetContentsReturnValue.Add(view);
            viewFactory.RaiseContentRegistered("MyRegion", view);

            Assert.Equal("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.Single(region.MockViews.Items);
            Assert.Equal(view, region.MockViews.Items[0]);
        }

        [Fact]
        public void NullRegionThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                var behavior = new AutoPopulateRegionBehavior(new MockRegionContentRegistry());

                behavior.Attach();
            });

        }

        [Fact]
        public void WhenSameViewTypeRegisteredTwice_RegionContainsSingleView()
        {
            var containerMock = new Mock<IContainerExtension>();
            ContainerLocator.SetContainerExtension(containerMock.Object);
            containerMock.Setup(c => c.Resolve(typeof(MockContentObject))).Returns(new MockContentObject());
            var registry = new RegionViewRegistry(containerMock.Object);
            registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));
            registry.RegisterViewWithRegion("MyRegion", typeof(MockContentObject));

            var region = new Region { Name = "MyRegion" };
            var behavior = new AutoPopulateRegionBehavior(registry)
            {
                Region = region
            };

            behavior.Attach();

            Assert.Single(region.Views);
        }

        [StaFact]
        public void ShouldAddDefaultViewByNameWhenRegionIsPopulated()
        {
            var containerMock = new Mock<IContainerExtension>();
            ContainerLocator.SetContainerExtension(containerMock.Object);
            var content = new object();
            containerMock.Setup(c => c.Resolve(typeof(object), "MyView")).Returns(content);
            var registry = new RegionViewRegistry(containerMock.Object);
            var host = new ContentControl();
            RegionManager.SetDefaultView(host, "MyView");

            var region = new Region { Name = "MyRegion" };
            var behavior = new AutoPopulateRegionBehavior(registry)
            {
                Region = region,
                HostControl = host
            };

            behavior.Attach();

            Assert.Single(region.Views);
            Assert.Same(content, region.Views.ElementAt(0));
        }

        [StaFact]
        public void WhenDefaultViewMatchesRegisteredView_RegionContainsSingleView()
        {
            var containerMock = new Mock<IContainerExtension>();
            ContainerLocator.SetContainerExtension(containerMock.Object);
            var content = new object();
            containerMock.Setup(c => c.Resolve(typeof(object), "MyView")).Returns(content);
            var registry = new RegionViewRegistry(containerMock.Object);
            registry.RegisterViewWithRegion("MyRegion", "MyView");
            var host = new ContentControl();
            RegionManager.SetDefaultView(host, "MyView");

            var region = new Region { Name = "MyRegion" };
            var behavior = new AutoPopulateRegionBehavior(registry)
            {
                Region = region,
                HostControl = host
            };

            behavior.Attach();

            Assert.Single(region.Views);
        }

        [Fact]
        public void CanAttachBeforeSettingName()
        {
            ContainerLocator.SetContainerExtension(Mock.Of<IContainerExtension>());
            var region = new MockPresentationRegion() { Name = null };
            var viewFactory = new MockRegionContentRegistry();
            var view = new object();
            viewFactory.GetContentsReturnValue.Add(view);
            var behavior = new AutoPopulateRegionBehavior(viewFactory)
            {
                Region = region
            };

            behavior.Attach();
            Assert.False(viewFactory.GetContentsCalled);

            region.Name = "MyRegion";

            Assert.True(viewFactory.GetContentsCalled);
            Assert.Equal("MyRegion", viewFactory.GetContentsArgumentRegionName);
            Assert.Single(region.MockViews.Items);
            Assert.Equal(view, region.MockViews.Items[0]);
        }

        private class MockRegionContentRegistry : IRegionViewRegistry
        {
            public readonly List<object> GetContentsReturnValue = new List<object>();
            public string GetContentsArgumentRegionName;
            public bool GetContentsCalled;

            public event EventHandler<ViewRegisteredEventArgs> ContentRegistered;

            public IEnumerable<object> GetContents(string regionName, IContainerProvider container)
            {
                GetContentsCalled = true;
                this.GetContentsArgumentRegionName = regionName;
                return this.GetContentsReturnValue;
            }

            public void RaiseContentRegistered(string regionName, object view)
            {
                this.ContentRegistered(this, new ViewRegisteredEventArgs(regionName, _ => view));
            }

            public void RegisterViewWithRegion(string regionName, Type viewType)
            {
                throw new NotImplementedException();
            }

            public void RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> getContentDelegate)
            {
                throw new NotImplementedException();
            }

            public void RegisterViewWithRegion(string regionName, string targetName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
