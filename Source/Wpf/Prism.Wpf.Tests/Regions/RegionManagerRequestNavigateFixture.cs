using System;
using System.Text;
using System.Collections.Generic;
using Xunit;

using Prism.Regions;

using Moq;

namespace Prism.Wpf.Tests.Regions
{
    
    public class RegionManagerRequestNavigateFixture
    {
        const string region = "Region";
        const string nonExistentRegion = "NonExistentRegion";
        const string source = "Source";

        private static Uri sourceUri = new Uri(source, UriKind.RelativeOrAbsolute);
        private static NavigationParameters parameters = new NavigationParameters();
        private static Action<NavigationResult> callback = (_) => { };

        private static Mock<IRegion> mockRegion;
        private static RegionManager regionManager;

        public RegionManagerRequestNavigateFixture()
        {
            mockRegion = new Mock<IRegion>();
            mockRegion.SetupGet((r) => r.Name).Returns(region);

            regionManager = new RegionManager();
            regionManager.Regions.Add(mockRegion.Object);
        }

        [Fact]
        public void ThrowsWhenNavigationCallbackIsNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, source, null, parameters)                
            );

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, source, navigationCallback: null)
            );

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, sourceUri, null, parameters)
            );

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, sourceUri, navigationCallback: null)
            );
        }

        [Fact]
        public void WhenNonExistentRegion_ReturnNavigationResultFalse()
        {
            NavigationResult result;

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
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, It.IsAny<Action<NavigationResult>>()));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionTarget()
        {
            regionManager.RequestNavigate(region, sourceUri);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, It.IsAny<Action<NavigationResult>>()));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceParameters()
        {
            regionManager.RequestNavigate(region, source, parameters);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, It.IsAny<Action<NavigationResult>>(), parameters));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceUriParameters()
        {
            regionManager.RequestNavigate(region, sourceUri, parameters);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, It.IsAny<Action<NavigationResult>>(), parameters));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceCallback()
        {
            regionManager.RequestNavigate(region, source, callback);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, callback));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionTargetCallback()
        {
            regionManager.RequestNavigate(region, sourceUri, callback);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, callback));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceCallbackParameters()
        {
            regionManager.RequestNavigate(region, source, callback, parameters);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, callback, parameters));
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceUriCallbackParameters()
        {
            regionManager.RequestNavigate(region, sourceUri, callback, parameters);
            mockRegion.Verify((r) => r.RequestNavigate(sourceUri, callback, parameters));
        }
    }
}
