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
        public static Queue<string> GetUriSegments(Uri uri)
        {
            Queue<string> segmentStack = new Queue<string>();
            string[] segments;

            if (!uri.IsAbsoluteUri)
                segments = EnsureAbsolute(uri).PathAndQuery.Split('/');
            else
                segments = uri.PathAndQuery.Split('/');

            for (int i = 0; i < segments.Length; i++)
            {
                var s = segments[i];
                if (string.IsNullOrEmpty(s))
                    continue;

                s = Uri.UnescapeDataString(s);
                segmentStack.Enqueue(s);
            }


            return segmentStack;
        }

        public static string GetSegmentName(string segment)
        {
            return segment.Split('?')[0];
        }

        public static NavigationParameters GetSegmentParameters(string segment)
        {
            string query = string.Empty;

            if (!String.IsNullOrWhiteSpace(segment))
            {
                var indexOfQuery = segment.IndexOf('?');
                if (indexOfQuery > 0)
                    query = segment.Substring(indexOfQuery);
            }

            return new NavigationParameters(query);
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
    }
}
