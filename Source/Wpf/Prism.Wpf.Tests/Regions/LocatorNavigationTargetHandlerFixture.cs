

using System;
using System.Windows;
using CommonServiceLocator;
using Xunit;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    
    public class LocatorNavigationTargetHandlerFixture
    {
        [Fact]
        public void WhenViewExistsAndDoesNotImplementINavigationAware_ThenReturnsView()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var view = new TestView();

            region.Add(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, returnedView);
        }

        [Fact]
        public void WhenRegionHasMultipleViews_ThenViewsWithMatchingTypeNameAreConsidered()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var view1 = new TestView();
            var view2 = "view";

            region.Add(view1);
            region.Add(view2);

            var navigationContext = new NavigationContext(null, new Uri(view2.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view2, returnedView);
        }

        [Fact]
        public void WhenRegionHasMultipleViews_ThenViewsWithMatchingFullTypeNameAreConsidered()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var view1 = new TestView();
            var view2 = "view";

            region.Add(view1);
            region.Add(view2);

            var navigationContext = new NavigationContext(null, new Uri(view2.GetType().FullName, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view2, returnedView);
        }

        [Fact]
        public void WhenViewExistsAndImplementsINavigationAware_ThenViewIsQueriedForNavigationAndIsReturnedIfAcceptsIt()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var viewMock = new Mock<INavigationAware>();
            viewMock
                .Setup(v => v.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(true)
                .Verifiable();

            region.Add(viewMock.Object);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(viewMock.Object, returnedView);
            viewMock.VerifyAll();
        }

        [StaFact]
        public void WhenViewExistsAndHasDataContextThatImplementsINavigationAware_ThenDataContextIsQueriedForNavigationAndIsReturnedIfAcceptsIt()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var dataContextMock = new Mock<INavigationAware>();
            dataContextMock
                .Setup(v => v.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(true)
                .Verifiable();
            var viewMock = new Mock<FrameworkElement>();
            viewMock.Object.DataContext = dataContextMock.Object;

            region.Add(viewMock.Object);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(viewMock.Object, returnedView);
            dataContextMock.VerifyAll();
        }

        [Fact]
        public void WhenNoCurrentMatchingViewExists_ThenReturnsNewlyCreatedInstanceWithServiceLocatorAddedToTheRegion()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var view = new TestView();

            serviceLocatorMock
                .Setup(sl => sl.GetInstance<object>(view.GetType().Name))
                .Returns(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, returnedView);
            Assert.True(region.Views.Contains(view));
        }

        [Fact]
        public void WhenViewExistsAndImplementsINavigationAware_ThenViewIsQueriedForNavigationAndNewInstanceIsCreatedIfItRejectsIt()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var viewMock = new Mock<INavigationAware>();
            viewMock
                .Setup(v => v.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(false)
                .Verifiable();

            region.Add(viewMock.Object);

            var newView = new TestView();

            serviceLocatorMock
                .Setup(sl => sl.GetInstance<object>(viewMock.Object.GetType().Name))
                .Returns(newView);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(newView, returnedView);
            Assert.True(region.Views.Contains(newView));
            viewMock.VerifyAll();
        }

        [StaFact]
        public void WhenViewExistsAndHasDataContextThatImplementsINavigationAware_ThenDataContextIsQueriedForNavigationAndNewInstanceIsCreatedIfItRejectsIt()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var dataContextMock = new Mock<INavigationAware>();
            dataContextMock
                .Setup(v => v.IsNavigationTarget(It.IsAny<NavigationContext>()))
                .Returns(false)
                .Verifiable();
            var viewMock = new Mock<FrameworkElement>();
            viewMock.Object.DataContext = dataContextMock.Object;

            region.Add(viewMock.Object);

            var newView = new TestView();

            serviceLocatorMock
                .Setup(sl => sl.GetInstance<object>(viewMock.Object.GetType().Name))
                .Returns(newView);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.Object.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(newView, returnedView);
            Assert.True(region.Views.Contains(newView));
            dataContextMock.VerifyAll();
        }

        [Fact]
        public void WhenViewCannotBeCreated_ThenThrowsAnException()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock
                .Setup(sl => sl.GetInstance<object>(typeof(TestView).Name))
                .Throws<ActivationException>();

            var region = new Region();

            var navigationContext = new NavigationContext(null, new Uri(typeof(TestView).Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            ExceptionAssert.Throws<InvalidOperationException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(region, navigationContext);

                });
        }

        [Fact]
        public void WhenViewAddedByHandlerDoesNotImplementINavigationAware_ThenReturnsView()
        {
            // Arrange

            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var view = new TestView();

            serviceLocatorMock
                .Setup(sl => sl.GetInstance<object>(view.GetType().Name))
                .Returns(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            // Act

            var firstReturnedView = navigationTargetHandler.LoadContent(region, navigationContext);
            var secondReturnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, firstReturnedView);
            Assert.Same(view, secondReturnedView);
            serviceLocatorMock.Verify(sl => sl.GetInstance<object>(view.GetType().Name), Times.Once());
        }

        [Fact]
        public void WhenRequestingContentForNullRegion_ThenThrows()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();

            var navigationContext = new NavigationContext(null, new Uri("/", UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(null, navigationContext);

                });
        }

        [Fact]
        public void WhenRequestingContentForNullContext_ThenThrows()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();

            var region = new Region();

            var navigationTargetHandler = new TestRegionNavigationContentLoader(serviceLocatorMock.Object);


            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(region, null);

                });
        }

        public class TestRegionNavigationContentLoader : RegionNavigationContentLoader
        {
            public TestRegionNavigationContentLoader(IServiceLocator serviceLocator)
                : base(serviceLocator)
            { }
        }

        public class TestView { }
    }
}
