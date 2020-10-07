using System;
using System.Collections.Generic;
using Prism.Common;
using Prism.Navigation;

namespace Prism.Regions.Navigation
{
    /// <summary>
    /// Encapsulates information about a navigation request.
    /// </summary>
    public class NavigationContext : INavigationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContext"/> class for a region name and a 
        /// <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="uri">The Uri.</param>
        public NavigationContext(IRegionNavigationService navigationService, Uri uri)
            : this(navigationService, uri, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationContext"/> class for a region name and a 
        /// <see cref="Uri"/>.
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="regionParameters">The navigation parameters.</param>
        /// <param name="uri">The Uri.</param>
        public NavigationContext(IRegionNavigationService navigationService, Uri uri, INavigationParameters regionParameters)
        {
            NavigationService = navigationService;
            Uri = uri;
            Parameters = regionParameters ?? new NavigationParameters();

            var queryString = uri != null ? UriParsingHelper.EnsureAbsolute(uri).Query : null;
            if (!string.IsNullOrEmpty(queryString))
            {
                UpdateRegionParameters(new NavigationParameters(queryString));
            }
        }

        /// <summary>
        /// Gets the region navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        public IRegionNavigationService NavigationService { get; private set; }

        /// <summary>
        /// Gets the navigation URI.
        /// </summary>
        /// <value>The navigation URI.</value>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the <see cref="INavigationParameters"/> extracted from the URI and the object parameters passed in navigation.
        /// </summary>
        /// <value>The URI query.</value>
        public INavigationParameters Parameters { get; }

        private void UpdateRegionParameters(INavigationParameters regionParameters)
        {
            foreach (KeyValuePair<string, object> parameter in regionParameters)
            {
                Parameters.Add(parameter.Key, parameter.Value);
            }
        }
    }
}
