using Prism.Navigation;
using System;
using System.Collections.Generic;

namespace Prism.Common
{
    /// <summary>
    /// Helper class for parsing <see cref="Uri"/> instances.
    /// </summary>
    public static class UriParsingHelper
    {
        private static readonly char[] _pathDelimiter = { '/' };

        public static Queue<string> GetUriSegments(Uri uri)
        {
            var segmentStack = new Queue<string>();

            if (!uri.IsAbsoluteUri)
            {
                uri = EnsureAbsolute(uri);
            }

            string[] segments = uri.PathAndQuery.Split(_pathDelimiter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                segmentStack.Enqueue(Uri.UnescapeDataString(segment));
            }

            return segmentStack;
        }

        public static string GetSegmentName(string segment)
        {
            return segment.Split('?')[0];
        }

        public static INavigationParameters GetSegmentParameters(string segment)
        {
            string query = string.Empty;

            if (string.IsNullOrWhiteSpace(segment))
            {
                return new NavigationParameters(query);
            }

            var indexOfQuery = segment.IndexOf('?');
            if (indexOfQuery > 0)
                query = segment.Substring(indexOfQuery);

            return new NavigationParameters(query);
        }

        public static INavigationParameters GetSegmentParameters(string uriSegment, INavigationParameters parameters)
        {
            var navParameters = UriParsingHelper.GetSegmentParameters(uriSegment);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in parameters)
                {
                    navParameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }

            return navParameters;
        }

        public static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            if (!uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost/" + uri, UriKind.Absolute);
            }
            return new Uri("http://localhost" + uri, UriKind.Absolute);
        }

        public static Uri Parse(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (uri.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost" + uri, UriKind.Absolute);
            }
            else
            {
                return new Uri(uri, UriKind.RelativeOrAbsolute);
            }
        }
    }
}
