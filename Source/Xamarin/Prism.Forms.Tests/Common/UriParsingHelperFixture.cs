﻿using Prism.Common;
using Prism.Navigation;
using System;
using Xunit;

namespace Prism.Forms.Tests.Common
{
    public class UriParsingHelperFixture
    {
        const string _relativeUri = "MainPage?id=3&name=brian";
        const string _absoluteUri = "htp://www.brianlagunas.com/MainPage?id=3&name=brian";
        const string _deepLinkAbsoluteUri = "android-app://HellowWorld/MainPage?id=1/ViewA?id=2/ViewB?id=3/ViewC?id=4";
        const string _deepLinkRelativeUri = "MainPage?id=1/ViewA?id=2/ViewB?id=3/ViewC?id=4";

        [Fact]
        public void ParametersParsedFromNullSegment()
        {
            var parameters = UriParsingHelper.GetSegmentParameters(null);
            Assert.NotNull(parameters);
        }

        [Fact]
        public void ParametersParsedFromEmptySegment()
        {
            var parameters = UriParsingHelper.GetSegmentParameters(string.Empty);
            Assert.NotNull(parameters);
        }

        [Fact]
        public void ParametersParsedFromRelativeUri()
        {
            var parameters = UriParsingHelper.GetSegmentParameters(_relativeUri);

            Assert.NotEmpty(parameters);

            Assert.Contains("id", parameters.Keys);
            Assert.Contains("name", parameters.Keys);

            Assert.Equal("3", parameters["id"]);
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void ParametersParsedFromAbsoluteUri()
        {
            var parameters = UriParsingHelper.GetSegmentParameters(_absoluteUri);

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

            var parameters = UriParsingHelper.GetSegmentParameters("MainPage" + navParameters.ToString());

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

            var parameters = UriParsingHelper.GetSegmentParameters("http://www.brianlagunas.com/MainPage" + navParameters.ToString());

            Assert.NotEmpty(parameters);

            Assert.Contains("id", parameters.Keys);
            Assert.Contains("name", parameters.Keys);

            Assert.Equal("3", parameters["id"]);
            Assert.Equal("brian", parameters["name"]);
        }

        [Fact]
        public void TargetNameParsedFromSingleSegment()
        {
            var target = UriParsingHelper.GetSegmentName(_relativeUri);
            Assert.Equal("MainPage", target);
        }

        [Fact]
        public void SegmentsParsedFromDeepLinkUri()
        {
            var target = UriParsingHelper.GetUriSegments(new Uri(_deepLinkAbsoluteUri));
            Assert.Equal(target.Count, 4);
        }

        [Fact]
        public void ParametersParsedFromDeepLinkAbsoluteUri()
        {
            var target = UriParsingHelper.GetUriSegments(new Uri(_deepLinkAbsoluteUri));
            Assert.Equal(target.Count, 4);

            var p1 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p1["id"], "1");

            var p2 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p2["id"], "2");

            var p3 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p3["id"], "3");

            var p4 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p4["id"], "4");
        }

        [Fact]
        public void ParametersParsedFromDeepLinkRelativeUri()
        {
            var target = UriParsingHelper.GetUriSegments(new Uri(_deepLinkRelativeUri, UriKind.Relative));
            Assert.Equal(target.Count, 4);

            var p1 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p1["id"], "1");

            var p2 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p2["id"], "2");

            var p3 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p3["id"], "3");

            var p4 = UriParsingHelper.GetSegmentParameters(target.Dequeue());
            Assert.Equal(p4["id"], "4");
        }

        [Fact]
        public void EnsureAbsoluteUriForRelativeUri()
        {
            var uri = UriParsingHelper.EnsureAbsolute(new Uri(_relativeUri, UriKind.Relative));
            Assert.True(uri.IsAbsoluteUri);
        }

        [Fact]
        public void EnsureAbsoluteUriForRelativeUriThatStartsWithSlash()
        {
            var uri = UriParsingHelper.EnsureAbsolute(new Uri("/" + _relativeUri, UriKind.Relative));
            Assert.True(uri.IsAbsoluteUri);
        }

        [Fact]
        public void EnsureAbsoluteUriForAbsoluteUri()
        {
            var uri = UriParsingHelper.EnsureAbsolute(new Uri(_absoluteUri, UriKind.Absolute));
            Assert.True(uri.IsAbsoluteUri);
        }

    }
}
