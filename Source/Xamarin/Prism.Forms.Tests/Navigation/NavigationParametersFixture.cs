using Prism.Navigation;
using System;
using Xunit;

namespace Prism.Forms.Tests.Navigation
{
    public class NavigationParametersFixture
    {
        const string _uri = "?id=3&name=brian";
        const string _uriWithNoQuestionMarkDelimiter = "id=3&name=brian";

        [Fact]
        public void ParametersParsedFromQuery()
        {
            var parameters = new NavigationParameters(_uri);
            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.ContainsKey("id"));
            Assert.Equal("3", parameters["id"]);
            Assert.True(parameters.ContainsKey("name"));
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromQueryWithNoQuestionMarkDelimiter()
        {
            var parameters = new NavigationParameters(_uriWithNoQuestionMarkDelimiter);
            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.ContainsKey("id"));
            Assert.Equal("3", parameters["id"]);
            Assert.True(parameters.ContainsKey("name"));
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void EmptyNavigationParametersWhenGivenNull()
        {
            var parameters = new NavigationParameters(null);
            Assert.NotNull(parameters);
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void EmptyNavigationParametersWhenGivenSpace()
        {
            var parameters = new NavigationParameters(" ");
            Assert.NotNull(parameters);
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void EmptyNavigationParametersWhenGivenNoValue()
        {
            var parameters = new NavigationParameters("id");
            Assert.NotNull(parameters);
            Assert.Equal(0, parameters.Count);
        }

        [Fact]
        public void WhenGivenDuplicateParametersThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var parameters = new NavigationParameters(_uri);
                parameters.Add("id", 3);
            });            
        }

        [Fact]
        public void GetValueSuccessfull()
        {
            const string validKey = "ValidKey";
            const int expectedValue = 100;

            var parameters = new NavigationParameters { { validKey, expectedValue } };
            var resultValue = parameters.GetValue<int>(validKey);

            Assert.Equal(expectedValue, resultValue);
        }

        [Fact]
        public void GetValueThrowsExceptionWhenCastIsInvalid()
        {
            const string invalidCastKey = "InvalidCastKey";
            const string invalidCastValue = "Invalid Cast Value";
            var expectedType = typeof(int);

            var parameters = new NavigationParameters { { invalidCastKey, "Blah" } };


            var ex = Assert.Throws<NavigationParameterInvalidCastException>(() => parameters.GetValue<int>(invalidCastKey));

            Assert.Equal(invalidCastKey, ex.InvalidCaskKey);
            Assert.Equal(expectedType, ex.ExpectedType);
            Assert.Equal(invalidCastValue.GetType(), ex.ActualType);
        }

        [Fact]
        public void GetValueThrowsExceptionWhenKeyIsMissing()
        {
            const string missingKey = "MissingKey";
            var parameters = new NavigationParameters();

            var ex = Assert.Throws<NavigationParameterNotFoundException>(() => parameters.GetValue<int>(missingKey));
            Assert.Equal(missingKey, ex.MissingKey);
        }
    }
}
