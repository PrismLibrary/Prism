

using System;

namespace Prism.Regions
{
    /// <summary>
    /// Argument class used by the <see cref="IRegionViewRegistry.ContentRegistered"/> event when a new content is registered.
    /// </summary>
    public class ViewRegisteredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the ViewRegisteredEventArgs class.
        /// </summary>
        /// <param name="regionName">The region name to which the content was registered.</param>
        /// <param name="getViewDelegate">The content which was registered.</param>
        public ViewRegisteredEventArgs(string regionName, Func<object> getViewDelegate)
        {
            this.GetView = getViewDelegate;
            this.RegionName = regionName;
        }

        /// <summary>
        /// Gets the region name to which the content was registered.
        /// </summary>
        public string RegionName { get; private set; }

        /// <summary>
        /// Gets the content which was registered.
        /// </summary>
        public Func<object> GetView { get; private set; }
    }
}
