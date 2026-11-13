#if IOS
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Prism.Common;
using UIKit;

namespace Prism.Platforms;

// MAUI forwards the native back button to the current ContentPage, not to
// PrismNavigationPage.OnBackButtonPressed. Intercept it before UIKit pops.
internal sealed class PrismNavigationRenderer : NavigationRenderer
{
    private bool _backPending;
    private bool _disposed;
    private int _managedPopDepth;
    private BackGestureDelegate _backGestureDelegate;

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        _backGestureDelegate = new BackGestureDelegate(this);
        InteractivePopGestureRecognizer.Delegate = _backGestureDelegate;
    }

    protected override async Task<bool> OnPopViewAsync(Page page, bool animated)
    {
        _managedPopDepth++;
        try { return await base.OnPopViewAsync(page, animated); }
        finally { _managedPopDepth--; }
    }

    protected override async Task<bool> OnPopToRoot(Page page, bool animated)
    {
        _managedPopDepth++;
        try { return await base.OnPopToRoot(page, animated); }
        finally { _managedPopDepth--; }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            if (InteractivePopGestureRecognizer is not null)
                InteractivePopGestureRecognizer.Delegate = null;
            _backGestureDelegate?.Dispose();
            _backGestureDelegate = null;
        }
        base.Dispose(disposing);
    }

    private sealed class BackGestureDelegate(PrismNavigationRenderer renderer) : UIGestureRecognizerDelegate
    {
        public override bool ShouldBegin(UIGestureRecognizer recognizer)
        {
            // Confirmation may be asynchronous. Keep both stacks intact until
            // Prism approves the request, then use its normal pop transition.
            renderer.RequestBack();
            return false;
        }
    }

    [Export("navigationBar:shouldPopItem:")]
    public bool ShouldPopNavigationItem(UINavigationBar navigationBar, UINavigationItem item)
    {
        // A managed pop updates the controller stack before its navigation bar.
        if (ViewControllers.Length < navigationBar.Items.Length ||
            _managedPopDepth > 0)
            return true;

        RequestBack();
        return false;
    }

    private void RequestBack()
    {
        if (_disposed || _managedPopDepth > 0)
            return;

        if (Element is not NavigationPage navigationPage || navigationPage.Navigation.NavigationStack.Count < 2)
            return;

        if (!_backPending)
        {
            var currentPage = navigationPage.CurrentPage;
            if (currentPage.SendBackButtonPressed())
                return;
            _backPending = true;
            // Let UIKit finish this callback before Prism starts a managed pop,
            // including when confirmation completes synchronously.
            BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    if (!_disposed && navigationPage.CurrentPage == currentPage)
                        await MvvmHelpers.HandleNavigationPageGoBack(navigationPage);
                }
                finally
                {
                    _backPending = false;
                }
            });
        }
    }
}
#endif
