using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
