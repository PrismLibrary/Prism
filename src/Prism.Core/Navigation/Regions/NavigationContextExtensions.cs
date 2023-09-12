using Prism.Common;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides extensions for the Navigation Context
    /// </summary>
    public static class NavigationContextExtensions
    {
        /// <summary>
        /// Gets the Navigation Segment name from the <see cref="NavigationContext"/>.
        /// </summary>
        /// <param name="context">The current instance of the <see cref="NavigationContext"/>.</param>
        /// <returns>The View Name that was navigated to.</returns>
        public static string NavigatedName(this NavigationContext context)
        {
            var uri = UriParsingHelper.EnsureAbsolute(context.Uri);
            var segments = UriParsingHelper.GetUriSegments(uri);
            var segment = segments.Dequeue();
            return UriParsingHelper.GetSegmentName(segment);
        }
    }
}
