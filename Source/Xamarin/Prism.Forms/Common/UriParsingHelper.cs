using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Common
{
    /// <summary>
    /// Helper class for parsing <see cref="Uri"/> instances.
    /// </summary>
    public static class UriParsingHelper
    {
        /// <summary>
        /// Gets the query part of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        public static string GetQuery(Uri uri)
        {
            return EnsureAbsolute(uri).Query;
        }

        /// <summary>
        /// Gets the AbsolutePath part of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        public static string GetAbsolutePath(Uri uri)
        {
            return EnsureAbsolute(uri).AbsolutePath;
        }

        /// <summary>
        /// Parses the query of <paramref name="uri"/> into a dictionary.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public static NavigationParameters GetParametersFromUri(Uri uri)
        {
            var query = GetQuery(uri);

            return new NavigationParameters(query);
        }

        public static Queue<string> GetSegmentStack(Uri uri)
        {
            Queue<string> segmentStack = new Queue<string>();

            var segments = uri.PathAndQuery.Split('/');

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

            var indexOfQuery = segment.IndexOf('?');
            if (indexOfQuery > 0)
                query = segment.Substring(indexOfQuery);

            return new NavigationParameters(query);
        }

        private static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            if ((uri != null) && !uri.OriginalString.StartsWith("/", StringComparison.Ordinal))
            {
                return new Uri("http://localhost/" + uri, UriKind.Absolute);
            }
            return new Uri("http://localhost" + uri, UriKind.Absolute);
        }
    }
}
