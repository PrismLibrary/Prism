using Microsoft.Practices.ServiceLocation;
using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;
using System;
using System.Reflection;
using Xunit;

namespace Prism.Forms.Tests.Common
{
    [Collection("ServiceLocator")]
    public class PageNavigationRegistryFixture : IDisposable
    {
        public PageNavigationRegistryFixture()
        {
            ServiceLocator.SetLocatorProvider(null);
        }

        [Fact]
        public void RegisterPageForNavigation()
        {
            ResetPageNavigationRegistry();

            var name = "MainPage";
            var type = typeof(PageMock);
            PageNavigationRegistry.Register(name, type);

            var info = PageNavigationRegistry.GetPageNavigationInfo(name);

            Assert.NotNull(info);
        }

        [Fact]
        public void NavigationINfoIsNullForUnregisteredPage()
        {
            var name = "UnRegisteredPage";
            var info = PageNavigationRegistry.GetPageNavigationInfo(name);

            Assert.Null(info);
        }

        [Fact]
        public void GetPageType()
        {
            ResetPageNavigationRegistry();

            var name = "MainPage";
            var type = typeof(PageMock);
            PageNavigationRegistry.Register(name, type);

            var infoType = PageNavigationRegistry.GetPageType(name);

            Assert.Equal(type, infoType);
        }

        [Fact]
        public void PageTypeIsNullForUnregisteredPage()
        {
            var name = "UnRegisteredPage";
            var infoType = PageNavigationRegistry.GetPageType(name);

            Assert.Null(infoType);
        }

        [Fact]
        public void GetPageNavigationOptions()
        {
            ResetPageNavigationRegistry();

            var name = "MainPage";
            var type = typeof(PageWithDefaultNavigationOptionsMock);
            PageNavigationRegistry.Register(name, type);

            var navOptions = PageNavigationRegistry.GetPageNavigationOptions(name);

            Assert.NotNull(navOptions);
        }

        [Fact]
        public void GetPageNavigationOptionsFromCache()
        {
            ResetPageNavigationRegistry();

            var name = "MainPage";
            var type = typeof(PageWithDefaultNavigationOptionsMock);
            PageNavigationRegistry.Register(name, type);
            var navOptions = PageNavigationRegistry.GetPageNavigationOptions(name);

            var name2 = "SecondPage";
            var type2 = typeof(PageWithDefaultNavigationOptionsMock);
            PageNavigationRegistry.Register(name2, type2);

            var navOptions2 = PageNavigationRegistry.GetPageNavigationOptions(name2);

            Assert.Equal(navOptions, navOptions2);
        }

        [Fact]
        public void PageNavigationOptionsAreNullForUnregisteredPage()
        {
            var name = "UnRegisteredPage";
            var navOptions = PageNavigationRegistry.GetPageNavigationOptions(name);

            Assert.Null(navOptions);
        }

        [Fact]
        public void GetPageNavigationProvider()
        {
            try
            {
                ResetPageNavigationRegistry();

                var serviceLocator = new ServiceLocatorMock(() => new PageNavigationProviderMock());
                ServiceLocator.SetLocatorProvider(() => serviceLocator);

                var name = "MainPage";
                var type = typeof(PageWithNavigationProviderMock);
                PageNavigationRegistry.Register(name, type);

                var provider = PageNavigationRegistry.GetPageNavigationProvider(name);

                Assert.NotNull(provider);
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(null);
            }
        }

        [Fact]
        public void GetPageNavigationProviderFromCache()
        {
            try
            {
                ResetPageNavigationRegistry();

                var serviceLocator = new ServiceLocatorMock(() => new PageNavigationProviderMock());
                ServiceLocator.SetLocatorProvider(() => serviceLocator);

                var name = "MainPage";
                var type = typeof(PageWithNavigationProviderMock);
                PageNavigationRegistry.Register(name, type);

                var provider = PageNavigationRegistry.GetPageNavigationProvider(name);

                Assert.NotNull(provider);

                var name2 = "SecondPage";
                var type2 = typeof(PageWithAllPageNavigationOptionsMock);
                PageNavigationRegistry.Register(name2, type2);

                var secondProvider = PageNavigationRegistry.GetPageNavigationProvider(name2);

                Assert.NotNull(secondProvider);

                Assert.Equal(provider, secondProvider);
            }
            finally
            {
                ServiceLocator.SetLocatorProvider(() => null);
            }
        }

        [Fact]
        public void InvalidPageNavigationProviderThrowsInvalidCastException()
        {
            ResetPageNavigationRegistry();

            Assert.Throws<InvalidCastException>(() =>
            {
                try
                {
                    var serviceLocator = new ServiceLocatorMock(() => new PageMock());
                    ServiceLocator.SetLocatorProvider(() => serviceLocator);

                    var name = "MainPage";
                    var type = typeof(PageWithInvalidPageNavigationProviderMock);
                    PageNavigationRegistry.Register(name, type);

                    var provider = PageNavigationRegistry.GetPageNavigationProvider(name);
                }
                finally
                {
                    ServiceLocator.SetLocatorProvider(null);
                }
            });
        }

        [Fact]
        public void PageNavigationProviderIsNullForPageWithNoProvider()
        {
            ResetPageNavigationRegistry();

            var name = "MainPage";
            var type = typeof(PageWithDefaultNavigationOptionsMock);
            PageNavigationRegistry.Register(name, type);

            var provider = PageNavigationRegistry.GetPageNavigationProvider(name);

            Assert.Null(provider);
        }

        [Fact]
        public void PageNavigationProviderIsNullForPageWithoutPageNavigationAttribute()
        {
            ResetPageNavigationRegistry();

            var name = "MainPage";
            var type = typeof(PageMock);
            PageNavigationRegistry.Register(name, type);
            var provider = PageNavigationRegistry.GetPageNavigationProvider(name);

            Assert.Null(provider);
        }

        [Fact]
        public void PageNavigationProviderIsNullForUnregisteredPage()
        {
            var name = "UnRegisteredPage";
            var provider = PageNavigationRegistry.GetPageNavigationProvider(name);

            Assert.Null(provider);
        }

        private static void ResetPageNavigationRegistry()
        {
            TypeInfo staticType = typeof(PageNavigationRegistry).GetTypeInfo();

            ConstructorInfo ci = null;

            foreach (var ctor in staticType.DeclaredConstructors)
            {
                ci = ctor;
                continue;
            }

            object[] parameters = new object[0];
            ci.Invoke(null, parameters);
        }

        public void Dispose()
        {
            ServiceLocator.SetLocatorProvider(null);
        }
    }
}
