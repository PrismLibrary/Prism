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
    public PrismNavigationPage()
    {
        BackButtonPressed += HandleBackButtonPressed;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PrismNavigationPage"/> with a specified <see cref="Page"/> at the Root
    /// </summary>
    /// <param name="page"></param>
    public PrismNavigationPage(Page page)
        : base(page)
    {
        BackButtonPressed += HandleBackButtonPressed;
    }

    /// <inheritdoc/>
    public event EventHandler BackButtonPressed;

    /// <inheritdoc/>
    protected override bool OnBackButtonPressed()
    {
        BackButtonPressed.Invoke(this, EventArgs.Empty);
        return false;
    }

    private async void HandleBackButtonPressed(object sender, EventArgs args)
    {
        await MvvmHelpers.HandleNavigationPageGoBack(this);
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

