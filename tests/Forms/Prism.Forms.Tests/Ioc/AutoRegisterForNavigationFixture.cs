using System;
using System.Collections.Generic;
using System.Text;
using Humanizer;
using MockApp;
using MockApp.Views;
using Moq;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Ioc
{
    public class AutoRegisterForNavigationFixture
    {
        public AutoRegisterForNavigationFixture()
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        [Theory]
        [InlineData(typeof(SomePage))]
        [InlineData(typeof(AnotherPage))]
        [InlineData(typeof(FooBarPage))]
        [InlineData(typeof(NavigationPage))]
        [InlineData(typeof(TabbedPage))]
        public void RegistersExpectedPagesWithTypeName(Type pageName)
        {
            var app = new MockPrismApp();
            app.MockContainer.Verify(x => x.Register(typeof(object), pageName, pageName.Name));
        }

        [Fact]
        public void DoesNotAutoRegisterAbstractPage()
        {
            var app = new MockPrismApp();

            app.MockContainer.Verify(x => x.Register(typeof(object), typeof(AbstractPage), nameof(AbstractPage)), Times.Never);
        }

        [Theory]
        [InlineData(typeof(SomePage), "somePage")]
        [InlineData(typeof(AnotherPage), "anotherPage")]
        [InlineData(typeof(FooBarPage), "fooBarPage")]
        [InlineData(typeof(NavigationPage), "navigationPage")]
        [InlineData(typeof(TabbedPage), "tabbedPage")]
        public void RegistersExpectedPagesWithCustomNames(Type pageName, string expectedName)
        {
            MockPrismApp.PageNameDelegate = t => t.Name.Camelize();
            var app = new MockPrismApp();
            app.MockContainer.Verify(x => x.Register(typeof(object), pageName, expectedName));
        }
    }
}
