using Prism.Common;

namespace Prism.Controls;

public class PrismNavigationPage : NavigationPage
{
    public PrismNavigationPage()
    {
        BackButtonPressed += HandleBackButtonPressed;
    }

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
