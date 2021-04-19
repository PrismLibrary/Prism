using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for the INavigationService to make decisions regarding a MasterDetailPage during navigation.
    /// </summary>
    [Obsolete("Xamarin.Forms now prefers use of the FlyoutPage. Please use IFlyoutPageOptions instead.")]
    public interface IMasterDetailPageOptions : IFlyoutPageOptions
    {
    }
}
