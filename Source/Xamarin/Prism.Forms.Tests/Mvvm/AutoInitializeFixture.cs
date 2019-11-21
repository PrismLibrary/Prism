using System;
using System.Collections.Generic;
using System.Text;
using Prism.Common;
using Prism.Forms.Tests.Mvvm.Mocks.ViewModels;
using Prism.Navigation;
using Xunit;

namespace Prism.Forms.Tests.Mvvm
{
    public class AutoInitializeFixture
    {
        [Fact]
        public void ThrowsAnExceptionWithoutRequiredParameter()
        {
            var vm = new AutoInitializedViewModelMock();
            var parameters = new NavigationParameters("?foo=bar");
            var ex = Record.Exception(() => PageUtilities.Abracadabra(vm, parameters));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public void NoExceptionWhenTitleIsProvided()
        {
            var vm = new AutoInitializedViewModelMock();
            var parameters = new NavigationParameters("?success=true&title=Hello");
            var ex = Record.Exception(() => PageUtilities.Abracadabra(vm, parameters));

            Assert.Null(ex);
            Assert.Equal("Hello", vm.Title);
        }

        [Fact]
        public void SetsBooleanFromQueryString()
        {
            var vm = new AutoInitializedViewModelMock();
            var parameters = new NavigationParameters("?success=true&title=Hello");
            var ex = Record.Exception(() => PageUtilities.Abracadabra(vm, parameters));

            Assert.Null(ex);
            Assert.True(vm.Success);
        }

        [Fact]
        public void SetsDateTimeFromQueryString()
        {
            var vm = new AutoInitializedViewModelMock();
            var parameters = new NavigationParameters("?someDate=July 11, 2019 08:00&title=Hello");
            var ex = Record.Exception(() => PageUtilities.Abracadabra(vm, parameters));

            Assert.Null(ex);
            var expected = new DateTime(2019, 7, 11, 8, 0, 0);
            Assert.Equal(expected, vm.SomeDate);
        }

        [Theory]
        [InlineData("status=OK", MockHttpStatus.OK)]
        [InlineData("status=201", MockHttpStatus.Created)]
        [InlineData("status=500", (MockHttpStatus)500)]
        public void SetsEnumFromQueryString(string queryString, MockHttpStatus status)
        {
            var vm = new AutoInitializedViewModelMock();
            var parameters = new NavigationParameters($"?{queryString}&title=Hello");
            var ex = Record.Exception(() => PageUtilities.Abracadabra(vm, parameters));

            Assert.Null(ex);
            Assert.Equal(status, vm.Status);
        }
    }
}
