using Prism.Dialogs;
using Prism.Navigation;

#nullable enable
namespace Prism.Common
{
    /// <summary>
    /// Helper class for parsing <see cref="Uri"/> instances.
    /// </summary>
    public static class UriParsingHelper
    {
        private static readonly char[] _pathDelimiter = ['/'];

        /// <summary>
        /// Gets the Uri segments from a deep linked Navigation Uri
        /// </summary>
        /// <param name="uri">A navigation <see cref="Uri"/>.</param>
        /// <returns>A collection of strings for each navigation segment within the Navigation <see cref="Uri"/>.</returns>
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

        /// <summary>
        /// Gets the Segment name from a Navigation Segment
        /// </summary>
        /// <param name="segment">A Navigation Segment</param>
        /// <returns>The navigation segment name from the provided segment.</returns>
        public static string GetSegmentName(string segment) => segment.Split('?')[0];

        /// <summary>
        /// Gets the Segment Parameters from a Navigation Segment that may contain a querystring
        /// </summary>
        /// <param name="segment">A navigation segment which may contain a querystring</param>
        /// <returns>The <see cref="INavigationParameters"/>.</returns>
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

        /// <summary>
        /// Gets Segment Parameters including those parameters from an existing <see cref="INavigationParameters"/> collection.
        /// </summary>
        /// <param name="uriSegment">The <see cref="Uri"/> segment</param>
        /// <param name="parameters">The existing <see cref="INavigationParameters"/>.</param>
        /// <returns>The combined <see cref="INavigationParameters"/>.</returns>
        public static INavigationParameters GetSegmentParameters(string uriSegment, INavigationParameters? parameters)
        {
            var navParameters = GetSegmentParameters(uriSegment);

            if (parameters is not null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in parameters)
                {
                    navParameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }

            return navParameters;
        }

        /// <summary>
        /// Gets the <see cref="IDialogParameters"/> from a specified segment
        /// </summary>
        /// <param name="segment">A navigation segment which may contain a querystring.</param>
        /// <returns>The <see cref="IDialogParameters"/>.</returns>
        public static IDialogParameters GetSegmentDialogParameters(string segment)
        {
            string query = string.Empty;

            if (string.IsNullOrWhiteSpace(segment))
            {
                return new DialogParameters(query);
            }

            var indexOfQuery = segment.IndexOf('?');
            if (indexOfQuery > 0)
                query = segment.Substring(indexOfQuery);

            return new DialogParameters(query);
        }

        /// <summary>
        /// Gets the combined <see cref="IDialogParameters"/> from a specified segment and existing <see cref="IDialogParameters"/>
        /// </summary>
        /// <param name="uriSegment">A navigation segment which may contain a querystring.</param>
        /// <param name="parameters">Existing <see cref="IDialogParameters"/>.</param>
        /// <returns></returns>
        public static IDialogParameters GetSegmentParameters(string uriSegment, IDialogParameters? parameters)
        {
            var dialogParameters = GetSegmentDialogParameters(uriSegment);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> navigationParameter in parameters)
                {
                    dialogParameters.Add(navigationParameter.Key, navigationParameter.Value);
                }
            }

            return dialogParameters;
        }

        /// <summary>
        /// Gets the query part of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        public static string GetQuery(Uri uri) => EnsureAbsolute(uri).Query;

        /// <summary>
        /// Gets the AbsolutePath part of <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        public static string GetAbsolutePath(Uri uri) => EnsureAbsolute(uri).AbsolutePath;

        /// <summary>
        /// Parses the query of <paramref name="uri"/> into a dictionary.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public static INavigationParameters ParseQuery(Uri uri)
        {
            var query = GetQuery(uri);

            return new NavigationParameters(query);
        }

        /// <summary>
        /// Parses a uri string to a properly initialized Uri for Prism
        /// </summary>
        /// <param name="uri">A uri string.</param>
        /// <returns>A <see cref="Uri"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws an <see cref="ArgumentNullException"/> when the string is null or empty.</exception>
        public static Uri Parse(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.StartsWith("/", StringComparison.Ordinal)
                ? new Uri("http://localhost" + uri, UriKind.Absolute)
                : new Uri(uri, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// This will provide the existing <see cref="Uri"/> if it is already Absolute, otherwise
        /// it will build a new Absolute <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The source <see cref="Uri"/>.</param>
        /// <returns>An Absolute <see cref="Uri"/>.</returns>
        public static Uri EnsureAbsolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                return uri;
            }

            return !uri.OriginalString.StartsWith("/", StringComparison.Ordinal) ? new Uri("http://localhost/" + uri, UriKind.Absolute) : new Uri("http://localhost" + uri, UriKind.Absolute);
        }
    }
}
