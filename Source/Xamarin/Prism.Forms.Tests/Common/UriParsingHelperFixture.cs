using Prism.Common;
using Prism.Navigation;
using System;
using Xunit;

namespace Prism.Forms.Tests.Common
{
    public class UriParsingHelperFixture
    {
        const string _relativeUri = "MainPage?id=3&name=brian";
        const string _absoluteUri = "htp://www.google.com/MainPage?id=3&name=brian";

        [Fact]
        public void ParametersParsedFromRelativeUri()
        {
            var uri = new Uri(_relativeUri, UriKind.Relative);
            var parameters = UriParsingHelper.ParseQuery(uri);

            Assert.NotEmpty(parameters);

            Assert.Contains("id", parameters.Keys);
            Assert.Contains("name", parameters.Keys);

            Assert.Equal("3", parameters["id"]);
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromAbsoluteUri()
        {
            var uri = new Uri(_absoluteUri, UriKind.Absolute);
            var parameters = UriParsingHelper.ParseQuery(uri);

            Assert.NotEmpty(parameters);

            Assert.Contains("id", parameters.Keys);
            Assert.Contains("name", parameters.Keys);

            Assert.Equal("3", parameters["id"]);
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromNavigationParametersInRelativeUri()
        {
            var navParameters = new NavigationParameters();
            navParameters.Add("id", 3);
            navParameters.Add("name", "brian");

            var uri = new Uri("MainPage" + navParameters.ToString(), UriKind.Relative);
            var parameters = UriParsingHelper.ParseQuery(uri);

            Assert.NotEmpty(parameters);

            Assert.Contains("id", parameters.Keys);
            Assert.Contains("name", parameters.Keys);

            Assert.Equal("3", parameters["id"]);
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromNavigationParametersInAbsoluteUri()
        {
            var navParameters = new NavigationParameters();
            navParameters.Add("id", 3);
            navParameters.Add("name", "brian");

            var uri = new Uri("http://www.google.com/MainPage" + navParameters.ToString(), UriKind.Absolute);
            var parameters = UriParsingHelper.ParseQuery(uri);

            Assert.NotEmpty(parameters);

            Assert.Contains("id", parameters.Keys);
            Assert.Contains("name", parameters.Keys);

            Assert.Equal("3", parameters["id"]);
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void TargetParsedFromRelativeUri()
        {
            var uri = new Uri(_relativeUri, UriKind.Relative);
            var target = UriParsingHelper.GetAbsolutePath(uri);

            Assert.Equal("/MainPage", target);
        }

        [Fact]
        public void TargetParsedFromAbsoluteUri()
        {
            var uri = new Uri(_absoluteUri, UriKind.Absolute);
            var target = UriParsingHelper.GetAbsolutePath(uri);

            Assert.Equal("/MainPage", target);
        }

        [Fact]
        public void GetParametersFromRelativeUri()
        {
            var uri = new Uri(_relativeUri, UriKind.Relative);
            var parameters = UriParsingHelper.GetQuery(uri);

            Assert.Equal("?id=3&name=brian", parameters);
        }

        [Fact]
        public void GetParametersFromAbsoluteUri()
        {
            var uri = new Uri(_absoluteUri, UriKind.Absolute);
            var parameters = UriParsingHelper.GetQuery(uri);

            Assert.Equal("?id=3&name=brian", parameters);
        }
    }
}
