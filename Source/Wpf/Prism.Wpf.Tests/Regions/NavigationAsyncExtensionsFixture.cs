

using System;
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
                    nv.RequestNavigateAsync(
                        It.Is<Uri>(u => !u.IsAbsoluteUri && u.OriginalString == "relative")))
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
                    nv.RequestNavigateAsync(
                        It.Is<Uri>(u => u.IsAbsoluteUri && u.Host == "test" && u.AbsolutePath == "/path")))
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
                    nv.RequestNavigateAsync(target))
                .Verifiable();


            navigateMock.Object.RequestNavigate(target);

            navigateMock.VerifyAll();
        }
    }
}
