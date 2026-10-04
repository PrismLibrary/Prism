#nullable enable
using Prism.Common;
using Prism.Tests.Common.Mocks;
using Xunit;

namespace Prism.Tests.Common
{
    public class ParametersFixture
    {
        [Fact]
        public void InterfaceEnumerationPreservesExplicitNullValues()
        {
            IParameters parameters = new MockParameters();
            parameters.Add("value", null);

            var entry = Assert.Single(parameters);
            Assert.Equal("value", entry.Key);
            Assert.Null(entry.Value);
            Assert.True(parameters.ContainsKey("value"));
            Assert.Null(parameters.GetValue<string>("value"));
            Assert.False(parameters.TryGetValue<string>("value", out var value));
            Assert.Null(value);
        }

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

            IEnumerable<MockEnum> values = [];

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetValueRunsStructParameterlessConstructorForMissingOrNullParameter(bool addNullParameter)
        {
            IParameters parameters = new MockParameters();
            if (addNullParameter)
                parameters.Add("value", null);

            Assert.Equal(42, parameters.GetValue<MockStructWithParameterlessConstructor>("value").Value);
            Assert.Equal(42, Assert.IsType<MockStructWithParameterlessConstructor>(parameters.GetValue("value", typeof(MockStructWithParameterlessConstructor))).Value);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 42)]
        public void TryGetValuePreservesMissingAndNullStructDefaults(bool addNullParameter, int expectedValue)
        {
            IParameters parameters = new MockParameters();
            if (addNullParameter)
                parameters.Add("value", null);

            var success = parameters.TryGetValue<MockStructWithParameterlessConstructor>("value", out var value);

            Assert.Equal(addNullParameter, success);
            Assert.Equal(expectedValue, value.Value);
        }

        [Fact]
        public void GetValuesRunsStructParameterlessConstructorForNullParameter()
        {
            IParameters parameters = new MockParameters();
            parameters.Add("value", null);

            var value = Assert.Single(parameters.GetValues<MockStructWithParameterlessConstructor>("value"));

            Assert.Equal(42, value.Value);
            Assert.Empty(parameters.GetValues<MockStructWithParameterlessConstructor>("missing"));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void NullableAndReferenceDefaultsRemainNullForMissingOrNullParameter(bool addNullParameter)
        {
            IParameters parameters = new MockParameters();
            if (addNullParameter)
                parameters.Add("value", null);

            Assert.Null(parameters.GetValue<MockStructWithParameterlessConstructor?>("value"));
            Assert.Null(parameters.GetValue("value", typeof(MockStructWithParameterlessConstructor?)));
            Assert.False(parameters.TryGetValue<MockStructWithParameterlessConstructor?>("value", out var nullableValue));
            Assert.Null(nullableValue);
            Assert.Empty(parameters.GetValues<MockStructWithParameterlessConstructor?>("value"));
            Assert.Null(parameters.GetValue<object>("value"));
            Assert.False(parameters.TryGetValue<object>("value", out var referenceValue));
            Assert.Null(referenceValue);
            Assert.Empty(parameters.GetValues<object>("value"));
        }

        [Fact]
        public void ConvertibleInterfaceDoesNotConvertPlainObject()
        {
            IParameters parameters = new MockParameters();
            parameters.Add("value", new object());

            Assert.False(parameters.TryGetValue<IConvertible>("value", out var value));
            Assert.Null(value);
            Assert.Empty(parameters.GetValues<IConvertible>("value"));
            var exception = Assert.Throws<InvalidCastException>(() => parameters.GetValue<IConvertible>("value"));
            Assert.Contains("Unable to convert the value of Type", exception.Message);
        }

        [Fact]
        public void ConvertibleInterfaceReturnsAssignableValue()
        {
            IParameters parameters = new MockParameters();
            object storedValue = 42;
            parameters.Add("value", storedValue);

            Assert.Same(storedValue, parameters.GetValue<IConvertible>("value"));
            Assert.True(parameters.TryGetValue<IConvertible>("value", out var value));
            Assert.Same(storedValue, value);
            Assert.Same(storedValue, Assert.Single(parameters.GetValues<IConvertible>("value")));
        }

        [Fact]
        public void ConvertibleTargetStillConvertsStringToInteger()
        {
            IParameters parameters = new MockParameters("value=42");

            Assert.Equal(42, parameters.GetValue<int>("value"));
            Assert.Equal(42, parameters.GetValue("value", typeof(int)));
            Assert.True(parameters.TryGetValue<int>("value", out var value));
            Assert.Equal(42, value);
            Assert.Equal(42, Assert.Single(parameters.GetValues<int>("value")));
        }
    }
}
