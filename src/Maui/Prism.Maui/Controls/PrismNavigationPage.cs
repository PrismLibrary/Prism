using Prism.Common;
using Prism.Navigation;
using UIModalPresentationStyle = Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.UIModalPresentationStyle;

namespace Prism.Controls;

/// <summary>
/// Provides a wrapper for the NavigationPage to better handle the OnBackButtonPressed event with Prism Navigation
/// </summary>
public class PrismNavigationPage : NavigationPage
{
    /// <summary>
    /// Creates a new instance of the <see cref="PrismNavigationPage"/>
    /// </summary>
    public PrismNavigationPage() { }

    /// <summary>
    /// Creates a new instance of the <see cref="PrismNavigationPage"/> with a specified <see cref="Page"/> at the Root
    /// </summary>
    /// <param name="page"></param>
    public PrismNavigationPage(Page page)
        : base(page)
    { }

    /// <inheritdoc/>
    protected sealed override bool OnBackButtonPressed()
    {
        MvvmHelpers.HandleNavigationPageGoBack(this).ConfigureAwait(false);
        return true; //Prism will always handle the navigation
    }

#if IOS
    /// <inheritdoc/>
    protected override async void OnDisappearing()
    {
        var presentationStyle = Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.GetModalPresentationStyle(this);

        if (presentationStyle != UIModalPresentationStyle.FullScreen && PageNavigationService.NavigationSource == PageNavigationSource.Device)
        {
            await MvvmHelpers.HandleNavigationPageSwipedAway(this);
        }

        base.OnDisappearing();
    }
#endif
}

