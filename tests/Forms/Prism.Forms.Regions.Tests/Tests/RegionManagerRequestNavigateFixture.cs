using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Prism.Navigation;
using Prism.Regions;
using Prism.Regions.Navigation;
using Xunit;

namespace Prism.Forms.Regions.Tests
{
    public class RegionManagerRequestNavigateFixture
    {
        const string region = "Region";
        const string nonExistentRegion = "NonExistentRegion";
        const string source = "Source";

        private static Uri sourceUri = new Uri(source, UriKind.RelativeOrAbsolute);
        private static INavigationParameters parameters = new NavigationParameters();
        private static Action<IRegionNavigationResult> callback = (_) => { };

        private static Mock<IRegion> mockRegion;
        private static Mock<IRegionNavigationService> mockNavigation;
        private static RegionManager regionManager;

        public RegionManagerRequestNavigateFixture()
        {
            mockNavigation = new Mock<IRegionNavigationService>();
            mockRegion = new Mock<IRegion>();
            mockRegion.SetupGet(x => x.NavigationService).Returns(mockNavigation.Object);
            mockRegion.SetupGet((r) => r.Name).Returns(region);

            regionManager = new RegionManager();
            regionManager.Regions.Add(mockRegion.Object);
        }

        [Fact]
        public void DoesNotThrowWhenNavigationCallbackIsNull()
        {
            var ex = Record.Exception(() => regionManager.RequestNavigate(region, source, null, parameters));
            Assert.Null(ex);

            ex = Record.Exception(() => regionManager.RequestNavigate(region, source, navigationCallback: null));
            Assert.Null(ex);

            ex = Record.Exception(() => regionManager.RequestNavigate(region, sourceUri, null, parameters));
            Assert.Null(ex);

            ex = Record.Exception(() => regionManager.RequestNavigate(region, sourceUri, navigationCallback: null));
            Assert.Null(ex);
        }

        [Fact]
        public void WhenNonExistentRegion_ReturnNavigationResultFalse()
        {
            IRegionNavigationResult result;

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, source, (r) => result = r, parameters);
            Assert.Equal(false, result.Result);

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, source, (r) => result = r);
            Assert.Equal(false, result.Result);

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, sourceUri, (r) => result = r, parameters);
            Assert.Equal(false, result.Result);

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, sourceUri, (r) => result = r);
            Assert.Equal(false, result.Result);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSource()
        {
            regionManager.RequestNavigate(region, source);
            mockNavigation.Verify((r) => r.RequestNavigate(sourceUri, It.IsAny<Action<IRegionNavigationResult>>(), null));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionTarget()
        {
            regionManager.RequestNavigate(region, sourceUri);
            mockNavigation.Verify((r) => r.RequestNavigate(sourceUri, It.IsAny<Action<IRegionNavigationResult>>(), null));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceParameters()
        {
            regionManager.RequestNavigate(region, source, parameters);
            mockRegion.Verify((r) => r.NavigationService.RequestNavigate(sourceUri, It.IsAny<Action<IRegionNavigationResult>>(), parameters));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceUriParameters()
        {
            regionManager.RequestNavigate(region, sourceUri, parameters);
            mockRegion.Verify((r) => r.NavigationService.RequestNavigate(sourceUri, It.IsAny<Action<IRegionNavigationResult>>(), parameters));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceCallback()
        {
            regionManager.RequestNavigate(region, source, callback);
            mockNavigation.Verify((r) => r.RequestNavigate(sourceUri, callback, null));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionTargetCallback()
        {
            regionManager.RequestNavigate(region, sourceUri, callback);
            mockNavigation.Verify((r) => r.RequestNavigate(sourceUri, callback, null));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceCallbackParameters()
        {
            regionManager.RequestNavigate(region, source, callback, parameters);
            mockRegion.Verify((r) => r.NavigationService.RequestNavigate(sourceUri, callback, parameters));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceUriCallbackParameters()
        {
            regionManager.RequestNavigate(region, sourceUri, callback, parameters);
            mockRegion.Verify((r) => r.NavigationService.RequestNavigate(sourceUri, callback, parameters));
        }
    }

    public static class ExceptionAssert
    {
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            Throws(typeof(TException), action);
        }

        public static void Throws(Type expectedExceptionType, Action action)
        {
            var ex = Record.Exception(action);
            Assert.IsType(expectedExceptionType, ex);
            //Assert.Fail("No exception thrown.  Expected exception type of {0}.", expectedExceptionType.Name);
        }
    }
}
