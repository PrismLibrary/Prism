using Prism.Common;
using Prism.Forms.Tests.Mocks.Views;
using System.Reflection;
using Xunit;

namespace Prism.Forms.Tests.Common
{
    public class PageNavigationRegistryFixture
    {
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
        public void NavigationInfoIsNullForUnregisteredPage()
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
    }
}
