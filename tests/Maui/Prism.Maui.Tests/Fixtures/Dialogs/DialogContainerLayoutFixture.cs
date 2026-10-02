using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Prism.Dialogs;
using Prism.Dialogs.Xaml;

namespace Prism.Maui.Tests.Fixtures.Dialogs;

public class DialogContainerLayoutFixture
{
    [Fact]
    public void MaskCoversSafeAreaWithoutChangingPopupPositioning()
    {
        var dialog = new ContentView();
        var mask = new BoxView();
        var bounds = new Rect(0.25, 0.75, 180, 120);
        DialogLayout.SetMask(dialog, mask);
        DialogLayout.SetLayoutBounds(dialog, bounds);
        var dismiss = new Command(() => { });

        var overlay = new LayoutContainer().CreateLayout(dialog, true, dismiss);

        Assert.Equal(SafeAreaEdges.None, overlay.SafeAreaEdges);
        Assert.Same(mask, overlay.Children[0]);
        var popupArea = Assert.IsType<AbsoluteLayout>(overlay.Children[1]);
        Assert.False(popupArea.IsSet(Layout.SafeAreaEdgesProperty));
        var popup = Assert.IsType<DialogContainerView>(Assert.Single(popupArea.Children));
        Assert.Same(dialog, popup.Content);
        Assert.Equal(bounds, AbsoluteLayout.GetLayoutBounds(popup));
        Assert.Equal(AbsoluteLayoutFlags.PositionProportional, AbsoluteLayout.GetLayoutFlags(popup));
        Assert.Same(dismiss, Assert.IsType<TapGestureRecognizer>(Assert.Single(mask.GestureRecognizers)).Command);
    }

    [Fact]
    public void DisabledMaskRetainsPopupAndDoesNotAddDismissGesture()
    {
        var dialog = new ContentView();
        var mask = new BoxView();
        DialogLayout.SetMask(dialog, mask);
        DialogLayout.SetUseMask(dialog, false);

        var overlay = new LayoutContainer().CreateLayout(dialog, true, new Command(() => { }));

        var popupArea = Assert.IsType<AbsoluteLayout>(Assert.Single(overlay.Children));
        Assert.Same(dialog, Assert.IsType<DialogContainerView>(Assert.Single(popupArea.Children)).Content);
        Assert.Empty(mask.GestureRecognizers);
    }

    private class LayoutContainer : DialogContainerPage
    {
        public Grid CreateLayout(View dialog, bool hideOnBackgroundTapped, ICommand dismiss) =>
            (Grid)GetContentLayout(new ContentPage(), dialog, hideOnBackgroundTapped, dismiss, new DialogParameters());
    }
}
