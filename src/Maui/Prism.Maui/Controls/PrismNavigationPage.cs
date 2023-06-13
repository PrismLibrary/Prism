using Prism.Common;

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
}
