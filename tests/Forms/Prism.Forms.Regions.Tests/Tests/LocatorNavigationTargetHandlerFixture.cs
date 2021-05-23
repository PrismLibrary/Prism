using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Prism.Ioc;
using Prism.Ioc.Internals;
using Prism.Regions;
using Prism.Regions.Navigation;
using Xamarin.Forms;
using Xunit;
using Region = Prism.Regions.Region;

namespace Prism.Forms.Regions.Tests
{
    public class LocatorNavigationTargetHandlerFixture
    {
        [Fact]
        public void WhenViewExistsAndDoesNotImplementIRegionAware_ThenReturnsView()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var region = new Region();

            var view = new TestView();

            region.Add(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, returnedView);
        }

        [Fact]
        public void WhenRegionHasMultipleViews_ThenViewsWithMatchingTypeNameAreConsidered()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var region = new Region();

            var view1 = new TestView();
            var view2 = new Grid();

            region.Add(view1);
            region.Add(view2);

            var navigationContext = new NavigationContext(null, new Uri(view2.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view2, returnedView);
        }

        [Fact]
        public void WhenRegionHasMultipleViews_ThenViewsWithMatchingFullTypeNameAreConsidered()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();
            //containerMock.As<IContainerInfo>()
            //    .Setup(x => x.GetRegistrationType("Xamarin.Forms.Grid"))
            //    .Returns(typeof(Grid));

            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var region = new Region();

            var view1 = new TestView();
            var view2 = new Grid();

            region.Add(view1);
            region.Add(view2);

            var navigationContext = new NavigationContext(null, new Uri(view2.GetType().FullName, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view2, returnedView);
        }

        [Fact]
        public void WhenViewExistsAndImplementsIRegionAware_ThenViewIsQueriedForNavigationAndIsReturnedIfAcceptsIt()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();

            var region = new Region();

            var viewMock = new Mock<View>();
            viewMock
                .As<IRegionAware>()
                .Setup(v => v.IsNavigationTarget(It.IsAny<INavigationContext>()))
                .Returns(true)
                .Verifiable();

            region.Add(viewMock.Object);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(viewMock.Object, returnedView);
            viewMock.VerifyAll();
        }

        [Fact]
        public void WhenViewExistsAndHasDataContextThatImplementsIRegionAware_ThenDataContextIsQueriedForNavigationAndIsReturnedIfAcceptsIt()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();

            var region = new Region();

            var bindingContextMock = new Mock<IRegionAware>();
            bindingContextMock
                .Setup(v => v.IsNavigationTarget(It.IsAny<INavigationContext>()))
                .Returns(true)
                .Verifiable();
            var viewMock = new Mock<View>();
            viewMock.Object.BindingContext = bindingContextMock.Object;

            region.Add(viewMock.Object);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(viewMock.Object, returnedView);
            bindingContextMock.VerifyAll();
        }

        [Fact]
        public void WhenNoCurrentMatchingViewExists_ThenReturnsNewlyCreatedInstanceWithServiceLocatorAddedToTheRegion()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();

            var region = new Region();

            var view = new TestView();

            containerMock.Setup(sl => sl.Resolve(typeof(object), view.GetType().Name)).Returns(view);
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, returnedView);
            Assert.True(region.Views.Contains(view));
        }

        [Fact]
        public void WhenViewExistsAndImplementsIRegionAware_ThenViewIsQueriedForNavigationAndNewInstanceIsCreatedIfItRejectsIt()
        {
            // Arrange

            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var region = new Region();

            var viewMock = new Mock<View>();
            viewMock
                .As<IRegionAware>()
                .Setup(v => v.IsNavigationTarget(It.IsAny<INavigationContext>()))
                .Returns(false)
                .Verifiable();

            region.Add(viewMock.Object);

            var newView = new TestView();

            containerMock.Setup(sl => sl.Resolve(typeof(object), viewMock.Object.GetType().Name)).Returns(newView);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(newView, returnedView);
            Assert.True(region.Views.Contains(newView));
            viewMock.VerifyAll();
        }

        [Fact]
        public void WhenViewExistsAndHasDataContextThatImplementsIRegionAware_ThenDataContextIsQueriedForNavigationAndNewInstanceIsCreatedIfItRejectsIt()
        {
            // Arrange
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var region = new Region();

            var bindingContextMock = new Mock<IRegionAware>();
            bindingContextMock
                .Setup(v => v.IsNavigationTarget(It.IsAny<INavigationContext>()))
                .Returns(false)
                .Verifiable();
            var viewMock = new Mock<View>();
            viewMock.Object.BindingContext = bindingContextMock.Object;

            region.Add(viewMock.Object);

            var newView = new TestView();

            containerMock.Setup(sl => sl.Resolve(typeof(object), viewMock.Object.GetType().Name)).Returns(newView);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);

            // Act
            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);

            // Assert
            Assert.Same(newView, returnedView);
            Assert.True(region.Views.Contains(newView));
            bindingContextMock.VerifyAll();
        }

        [Fact]
        public void WhenViewCannotBeCreated_ThenThrowsAnException()
        {
            var containerMock = new Mock<IContainerExtension>();
            containerMock.Setup(x => x.Resolve(typeof(object), typeof(TestView).Name))
                .Throws(new ContainerResolutionException(typeof(object), typeof(TestView).Name, null));
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var region = new Region();

            var navigationContext = new NavigationContext(null, new Uri(typeof(TestView).Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);

            // Act
            var ex = Record.Exception(() => navigationTargetHandler.LoadContent(region, navigationContext));
            Assert.IsType<ContainerResolutionException>(ex);
        }

        [Fact]
        public void WhenViewAddedByHandlerDoesNotImplementIRegionAware_ThenReturnsView()
        {
            // Arrange
            var containerMock = new Mock<IContainerExtension>();

            var region = new Region();

            var view = new Grid();

            containerMock.Setup(x => x.Resolve(typeof(object), view.GetType().Name)).Returns(view);
            containerMock.Setup(x => x.Resolve(typeof(IActiveRegionHelper)))
                .Returns(new RegionResolverOverrides());

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);

            // Act
            var firstReturnedView = navigationTargetHandler.LoadContent(region, navigationContext);
            var secondReturnedView = navigationTargetHandler.LoadContent(region, navigationContext);

            // Assert
            Assert.Same(view, firstReturnedView);
            Assert.Same(view, secondReturnedView);
            containerMock.Verify(sl => sl.Resolve(typeof(object), view.GetType().Name), Times.Once());
        }

        [Fact]
        public void WhenRequestingContentForNullRegion_ThenThrows()
        {
            var containerMock = new Mock<IContainerExtension>();

            var navigationContext = new NavigationContext(null, new Uri("/", UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(null, navigationContext);

                });
        }

        [Fact]
        public void WhenRequestingContentForNullContext_ThenThrows()
        {
            var containerMock = new Mock<IContainerExtension>();

            var region = new Region();

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock.Object);


            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(region, null);

                });
        }

        public class TestRegionNavigationContentLoader : RegionNavigationContentLoader
        {
            public TestRegionNavigationContentLoader(IContainerExtension container)
                : base(container)
            { }
        }

        public class TestView : Xamarin.Forms.View { }
    }

    public class ActivationException : Exception
    {
        public ActivationException()
        {
        }
    }
}
