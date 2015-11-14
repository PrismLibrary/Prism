

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    [TestClass]
    public class NavigationAsyncExtensionsFixture
    {
        [TestMethod]
        public void WhenNavigatingWithANullThis_ThenThrows()
        {
            INavigateAsync navigate = null;
            string target = "";

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigate.RequestNavigate(target);
                });
        }

        [TestMethod]
        public void WhenNavigatingWithANullStringTarget_ThenThrows()
        {
            INavigateAsync navigate = new Mock<INavigateAsync>().Object;
            string target = null;

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigate.RequestNavigate(target);
                });
        }

        [TestMethod]
        public void WhenNavigatingWithARelativeStringTarget_ThenNavigatesToRelativeUri()
        {
            var navigateMock = new Mock<INavigateAsync>();
            navigateMock
                .Setup(nv =>
                    nv.RequestNavigate(
                        It.Is<Uri>(u => !u.IsAbsoluteUri && u.OriginalString == "relative"),
                        It.Is<Action<NavigationResult>>(c => c != null)))
                .Verifiable();

            string target = "relative";

            navigateMock.Object.RequestNavigate(target);

            navigateMock.VerifyAll();
        }

        [TestMethod]
        public void WhenNavigatingWithAnAbsoluteStringTarget_ThenNavigatesToAbsoluteUri()
        {
            var navigateMock = new Mock<INavigateAsync>();
            navigateMock
                .Setup(nv =>
                    nv.RequestNavigate(
                        It.Is<Uri>(u => u.IsAbsoluteUri && u.Host == "test" && u.AbsolutePath == "/path"),
                        It.Is<Action<NavigationResult>>(c => c != null)))
                .Verifiable();

            string target = "http://test/path";

            navigateMock.Object.RequestNavigate(target);

            navigateMock.VerifyAll();
        }

        [TestMethod]
        public void WhenNavigatingWithANullThisAndAUri_ThenThrows()
        {
            INavigateAsync navigate = null;
            Uri target = new Uri("test", UriKind.Relative);

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigate.RequestNavigate(target);
                });
        }

        [TestMethod]
        public void WhenNavigatingWithAUri_ThenNavigatesToUriWithCallback()
        {
            Uri target = new Uri("relative", UriKind.Relative);

            var navigateMock = new Mock<INavigateAsync>();
            navigateMock
                .Setup(nv =>
                    nv.RequestNavigate(
                        target,
                        It.Is<Action<NavigationResult>>(c => c != null)))
                .Verifiable();


            navigateMock.Object.RequestNavigate(target);

            navigateMock.VerifyAll();
        }

        [TestMethod]
        public async Task WhenNavigatingAsync_NavigationResultIsReturned()
        {
            string target = "relative";

            var navServiceMock = new Mock<IRegionNavigationService>();
            var navigateMock = new Mock<INavigateAsync>();
            navigateMock
                .Setup(nv =>
                    nv.RequestNavigateAsync(
                        It.Is<Uri>(u => string.Equals(u.OriginalString, target))))
                .ReturnsAsync(new NavigationResult(new NavigationContext(navServiceMock.Object, new Uri(target, UriKind.Relative)), true))
                .Verifiable();

            var result = await navigateMock.Object.RequestNavigateAsync(target);

            navigateMock.VerifyAll();

            Assert.AreEqual(true, result.Result);
            Assert.AreEqual(target, result.Context.Uri.OriginalString);
            Assert.IsNull(result.Error);
        }
    }
}
