using System;
using Xamarin.Forms;

namespace Prism.Services
{
    public interface IAppLinkService
    {
        /// <summary>
        /// Add the provided application link to the application index
        /// </summary>
        /// <param name="entry">Application link details</param>
        void RegisterLink(IAppLinkEntry entry);

        /// <summary>
        /// Removes the provided application link from the application index
        /// </summary>
        /// <param name="entry">Application link details</param>
        void DeregisterLink(IAppLinkEntry entry);

        /// <summary>
        /// Removes the provided application link from the application index
        /// </summary>
        /// <param name="uri">The URI of your registered application link</param>
        void DeregisterLink(Uri uri);
    }
}
