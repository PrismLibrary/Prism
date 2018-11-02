

using System;
using Xunit;
using Moq;
using Prism.Regions;

namespace Prism.Wpf.Tests.Regions
{
    
    public class NavigationAsyncExtensionsFixture
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
    }
}
