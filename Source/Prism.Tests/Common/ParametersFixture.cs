using System.Collections.Generic;
using System.Linq;
using Prism.Tests.Common.Mocks;
using Xunit;

namespace Prism.Tests.Common
{
    public class ParametersFixture
    {
        [Fact]
        public void TryGetValueOfT()
        {
            var parameters = new MockParameters("mock=Foo&mock2=1");
            bool success = false;
            MockEnum value = default;
            MockEnum value1 = default;

            var ex = Record.Exception(() => success = parameters.TryGetValue<MockEnum>("mock", out value));
            var ex2 = Record.Exception(() => success = parameters.TryGetValue<MockEnum>("mock2", out value1));
            Assert.Null(ex);
            Assert.True(success);
            Assert.Equal(MockEnum.Foo, value);
            Assert.Equal(value, value1);
        }

        [Fact]
        public void GetValuesOfT()
        {
            var parameters = new MockParameters("mock=Foo&mock=2&mock=Fizz");

            IEnumerable<MockEnum> values = default;

            var ex = Record.Exception(() => values = parameters.GetValues<MockEnum>("mock"));
            Assert.Null(ex);
            Assert.Equal(3, values.Count());
            Assert.Equal(MockEnum.Foo, values.ElementAt(0));
            Assert.Equal(MockEnum.Bar, values.ElementAt(1));
            Assert.Equal(MockEnum.Fizz, values.ElementAt(2));
        }


        [Fact]
        public void GetValue()
        {
            var parameters = new MockParameters("mock=Foo&mock1=2&mock2=Fizz");
            MockEnum value = default;
            MockEnum value1 = default;

            var ex = Record.Exception(() => value = parameters.GetValue<MockEnum>("mock"));
            var ex2 = Record.Exception(() => value1 = parameters.GetValue<MockEnum>("mock1"));
            Assert.Null(ex);
            Assert.Equal(MockEnum.Foo, value);
            Assert.Equal(MockEnum.Bar, value1);
        }
    }
}
