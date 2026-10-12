#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Prism.Dialogs;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Xaml;
using NavigationMode = Prism.Navigation.NavigationMode;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.Common;

/// <summary>Dispatches navigation and lifecycle callbacks to views, view models, and child regions.</summary>
public static class MvvmHelpers
{
    /// <summary>Invokes an action on each matching view, binding context, and child region view.</summary>
    /// <typeparam name="T">The callback contract to find.</typeparam>
    /// <param name="view">The view to inspect; null produces no callbacks.</param>
    /// <param name="action">The action invoked for each matching object.</param>
    /// <remarks>A view and its binding context are evaluated separately. Child regions are visited recursively after both.</remarks>
    public static void InvokeViewAndViewModelAction<T>(object? view, Action<T> action) where T : class
    {
        if (view is T viewAsT)
        {
            action(viewAsT);
        }

        if (view is BindableObject { BindingContext: T viewModelAsT })
        {
            action(viewModelAsT);
        }

        if (view is Page page)
        {
            var children = page.GetChildRegions();
            if (children is not null)
                foreach (var child in children)
                    InvokeViewAndViewModelAction<T>(child, action);
        }
    }

    /// <summary>Sequentially awaits an action on matching views, binding contexts, and child region views.</summary>
    /// <typeparam name="T">The callback contract to find.</typeparam>
    /// <param name="view">The view to inspect; null produces no callbacks.</param>
    /// <param name="action">The asynchronous action invoked for each matching object.</param>
    /// <returns>A task completing after all matching callbacks finish.</returns>
    public static async Task InvokeViewAndViewModelActionAsync<T>(object? view, Func<T, Task> action) where T : class
    {
        if (view is T viewAsT)
        {
            await action(viewAsT);
        }

        if (view is BindableObject { BindingContext: T viewModelAsT })
        {
            await action(viewModelAsT);
        }

        if (view is Page page)
        {
            var children = page.GetChildRegions();
            if (children is not null)
                foreach (var child in children)
                    await InvokeViewAndViewModelActionAsync(child, action);
        }
    }

    /// <summary>Destroys child pages and notifies destructible views and view models before scheduling visual cleanup.</summary>
    /// <param name="view">The view to destroy.</param>
    /// <remarks>Visual behaviors and the binding context are cleared after a dispatcher delay to allow transition animations to complete.</remarks>
    public static void DestroyPage(IView? view)
    {
        try
        {
            DestroyChildren(view);

            InvokeViewAndViewModelAction<IDestructible>(view, v => v.Destroy());

            //I'm actually not sure if this is necessary, but it seems like a good idea to clear the child regions of the page before we clear the behaviors and binding context of the page itself.
            if (view is Page page)
                page.ClearChildRegions();

            if (view is VisualElement visualElement)
            {
                DeferredCleanup(visualElement);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot destroy {view}.", ex);
        }
    }

    private static void DeferredCleanup(VisualElement visualElement)
    {
        // Delay cleanup until after page transition animations have completed.
        // We cannot clear BindingContext immediately because the page is still
        // visible during the pop animation, causing a visual flicker.
        // Neither Unloaded nor ParentChanged fire reliably across all platforms,
        // so a dispatcher delay is the only consistent approach.
        visualElement.Dispatcher?.DispatchDelayed(TimeSpan.FromMilliseconds(800), () =>
        {
            visualElement.Behaviors?.Clear();
            visualElement.BindingContext = null;
        });
    }

    private static void DestroyChildren(IView? page)
    {
        switch (page)
        {
            case FlyoutPage flyout:
                DestroyPage(flyout.Flyout);
                DestroyPage(flyout.Detail);
                break;
            case TabbedPage tabbedPage:
                foreach (var item in tabbedPage.Children.Reverse())
                {
                    DestroyPage(item);
                }

                break;
            case NavigationPage navigationPage:
                foreach (var item in navigationPage.Navigation.NavigationStack.Reverse())
                {
                    DestroyPage(item);
                }

                break;
        }
    }

    /// <summary>Destroys modal pages in reverse stack order, then destroys the underlying page.</summary>
    /// <param name="page">The underlying page to destroy last.</param>
    /// <param name="modalStack">The modal pages ordered from bottom to top.</param>
    public static void DestroyWithModalStack(Page? page, IList<Page?> modalStack)
    {
        foreach (var childPage in modalStack.Reverse())
        {
            DestroyPage(childPage);
        }

        DestroyPage(page);
    }

    /// <summary>Finds a callback implementation on a view or its binding context.</summary>
    /// <typeparam name="T">The callback contract to find.</typeparam>
    /// <param name="view">The view to inspect.</param>
    /// <returns>The view when it implements the contract, otherwise its matching binding context, or null.</returns>
    public static T? GetImplementerFromViewOrViewModel<T>(object view)
        where T : class
    {
        if (view is T viewAsT)
        {
            return viewAsT;
        }

        if (view is VisualElement { BindingContext: T vmAsT })
        {
            return vmAsT;
        }

        return null;
    }

    /// <summary>Determines whether an existing region view can handle a navigation request.</summary>
    /// <param name="view">The existing region view.</param>
    /// <param name="navigationContext">The requested region navigation.</param>
    /// <returns>The region-aware callback result, or whether the route matches the view's type name when no callback is present.</returns>
    public static bool IsNavigationTarget(object view, NavigationContext navigationContext)
    {
        var implementor = GetImplementerFromViewOrViewModel<IRegionAware>(view);
        if (implementor is not null)
            return implementor.IsNavigationTarget(navigationContext);

        var uri = navigationContext.Uri;
        if (!uri.IsAbsoluteUri)
            uri = new Uri(new Uri("app://prism.regions"), uri);
        var path = uri.LocalPath.Substring(1);
        var viewType = view.GetType();

        return path == viewType.Name || path == viewType.FullName;
    }

    /// <summary>Notifies region-aware views and view models that navigation is leaving the view.</summary>
    /// <param name="view">The departing view.</param>
    /// <param name="navigationContext">The region navigation request.</param>
    public static void OnNavigatedFrom(object? view, NavigationContext navigationContext)
    {
        InvokeViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedFrom(navigationContext));
    }

    /// <summary>Notifies region-aware views and view models that navigation has reached the view.</summary>
    /// <param name="view">The destination view.</param>
    /// <param name="navigationContext">The region navigation request.</param>
    public static void OnNavigatedTo(object? view, NavigationContext navigationContext)
    {
        InvokeViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedTo(navigationContext));
    }

    /// <summary>Asks the page or its view model to confirm navigation asynchronously.</summary>
    /// <param name="page">The page whose confirmation contract is inspected.</param>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>The asynchronous confirmation result, falling back to synchronous confirmation or true when neither contract is implemented.</returns>
    public static Task<bool> CanNavigateAsync(object page, INavigationParameters parameters)
    {
        var implementer = GetImplementerFromViewOrViewModel<IConfirmNavigationAsync>(page);
        if (implementer is null)
            return Task.FromResult(CanNavigate(page, parameters));

        return implementer.CanNavigateAsync(parameters);
    }

    /// <summary>Asks the page or its view model to confirm navigation synchronously.</summary>
    /// <param name="page">The page whose confirmation contract is inspected.</param>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>The confirmation result, or true when no synchronous confirmation contract is implemented.</returns>
    public static bool CanNavigate(object page, INavigationParameters parameters)
    {
        var implementer = GetImplementerFromViewOrViewModel<IConfirmNavigation>(page);
        return implementer?.CanNavigate(parameters) ?? true;
    }

    /// <summary>Notifies page navigation callbacks that navigation is leaving the page.</summary>
    /// <param name="page">The departing page.</param>
    /// <param name="parameters">The navigation parameters.</param>
    public static void OnNavigatedFrom(object? page, INavigationParameters parameters)
    {
        if (page != null)
            InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
    }

    /// <summary>Invokes synchronous initialization callbacks, then awaits asynchronous initialization callbacks.</summary>
    /// <param name="page">The page to initialize, including matching view models and child region views.</param>
    /// <param name="parameters">The initialization parameters.</param>
    /// <returns>A task completing after all initialization callbacks finish.</returns>
    public static async Task OnInitializedAsync(object? page, INavigationParameters parameters)
    {
        if (page is null) return;

        InvokeViewAndViewModelAction<IInitialize>(page, v => v.Initialize(parameters));
        await InvokeViewAndViewModelActionAsync<IInitializeAsync>(page, async v => await v.InitializeAsync(parameters));
    }

    private static bool HasKey(this IEnumerable<KeyValuePair<string, object>> parameters, string name, [MaybeNullWhen(returnValue: false)] out string key)
    {
        key = parameters.Select(x => x.Key).FirstOrDefault(k => k.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        return !string.IsNullOrEmpty(key);
    }

    /// <summary>Notifies page navigation callbacks that navigation has reached the page.</summary>
    /// <param name="page">The destination page.</param>
    /// <param name="parameters">The navigation parameters.</param>
    public static void OnNavigatedTo(object? page, INavigationParameters parameters)
    {
        if (page != null)
            InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
    }

    /// <summary>Finds the page to notify when back navigation leaves the specified page.</summary>
    /// <param name="page">The departing page.</param>
    /// <param name="mainPage">The window's underlying page, used when the modal stack has no preceding page.</param>
    /// <param name="useModalNavigation">Whether to search the modal stack first.</param>
    /// <returns>The destination's active child page, or null when no destination is available.</returns>
    public static Page? GetOnNavigatedToTarget(Page page, IView? mainPage, bool useModalNavigation)
    {
        Page? target;
        if (useModalNavigation)
        {
            var previousPage = GetPreviousPage(page, page.Navigation.ModalStack);

            //MainPage is not included in the navigation stack, so if we can't find the previous page above
            //let's assume they are going back to the MainPage
            target = GetOnNavigatedToTargetFromChild(previousPage ?? mainPage);
        }
        else
        {
            target = GetPreviousPage(page, page.Navigation.NavigationStack);
            if (target != null)
                target = GetOnNavigatedToTargetFromChild(target);
            else
                target = GetOnNavigatedToTarget(page, mainPage, true);
        }

        return target;
    }

    /// <summary>Descends through flyout detail, selected tabs, and navigation stacks to find the active child page.</summary>
    /// <param name="target">The destination view to inspect.</param>
    /// <returns>The active child page, or null if the destination is not a page.</returns>
    public static Page? GetOnNavigatedToTargetFromChild(IView? target)
    {
        Page? child = null;

        if (target is FlyoutPage flyout)
            child = flyout.Detail;
        else if (target is TabbedPage tabbed)
            child = tabbed.CurrentPage;
        else if (target is NavigationPage np)
            child = np.Navigation.NavigationStack.Last();

        if (child != null)
            target = GetOnNavigatedToTargetFromChild(child);

        if (target is Page page)
            return page;

        return null;
    }

    /// <summary>Gets the page preceding the current page in a navigation stack.</summary>
    /// <param name="currentPage">The current page to locate.</param>
    /// <param name="navStack">The stack ordered from root to current page.</param>
    /// <returns>The preceding page, or null when no preceding index exists.</returns>
    /// <remarks>If the current page is absent, the final stack entry is treated as its position.</remarks>
    public static Page? GetPreviousPage(Page currentPage, IReadOnlyList<Page?> navStack)
    {
        Page? previousPage = null;

        int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
        int previousPageIndex = currentPageIndex - 1;
        if (navStack.Count >= 0 && previousPageIndex >= 0)
            previousPage = navStack[previousPageIndex];

        return previousPage;
    }

    /// <summary>Finds the position of a page in a navigation stack.</summary>
    /// <param name="currentPage">The page to locate.</param>
    /// <param name="navStack">The navigation stack to search.</param>
    /// <returns>The matching index, or the final stack index when the page is absent; -1 for an empty stack.</returns>
    public static int GetCurrentPageIndex(Page currentPage, IReadOnlyList<Page?> navStack)
    {
        int stackCount = navStack.Count;
        for (int x = 0; x < stackCount; x++)
        {
            var view = navStack[x];
            if (view == currentPage)
                return x;
        }

        return stackCount - 1;
    }

    /// <summary>Gets the currently displayed page using the configured page resolver.</summary>
    /// <param name="mainPage">The window's root page.</param>
    /// <returns>The active page returned by the resolver.</returns>
    /// <remarks>The default resolver considers modal pages, selected tabs, flyout detail, and navigation stacks.</remarks>
    public static Page? GetCurrentPage(Page mainPage) =>
        _getCurrentPage(mainPage);

    /// <summary>Replaces the resolver used to determine the currently displayed page.</summary>
    /// <param name="getCurrentPageDelegate">The resolver receiving a window's root page.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetCurrentPageDelegate(Func<Page, Page?> getCurrentPageDelegate) =>
        _getCurrentPage = getCurrentPageDelegate;

    private static Func<Page, Page?> _getCurrentPage = mainPage =>
    {
        var page = mainPage;

        var lastModal = page.Navigation.ModalStack.LastOrDefault();
        if (lastModal != null)
            page = lastModal;

        return EvaluateCurrentPage(page);
    };

    internal static Page? GetTarget(Page? target)
    {
        return target switch
        {
            FlyoutPage flyout => GetTarget(flyout.Detail),
            TabbedPage tabbed => GetTarget(tabbed.CurrentPage),
            NavigationPage navigation => GetTarget(navigation.CurrentPage) ?? navigation,
            ContentPage page => page,
            null => null,
            _ => throw new NotSupportedException($"The page type '{target.GetType().FullName}' is not supported.")
        };
    }

    private static Page? EvaluateCurrentPage(Page? target)
    {
        Page? child = GetTarget(target);

        if (child is not null)
            target = GetOnNavigatedToTargetFromChild(child);

        if (target is { } page)
        {
            if (target is IDialogContainer)
            {
                return page.Parent switch
                {
                    Page parentPage => GetTarget(parentPage),
                    Window window => GetTarget(window.Page),
                    _ => throw new InvalidOperationException("Unable to determine the current page.")
                };
            }

            return page.Parent switch
            {
                TabbedPage tab when tab.CurrentPage != target => EvaluateCurrentPage(tab.CurrentPage),
                NavigationPage nav when nav.CurrentPage != target => EvaluateCurrentPage(nav.CurrentPage),
                _ => target
            };
        }

        return null;
    }

    /// <summary>Routes a platform back request through Prism navigation confirmation and callbacks.</summary>
    /// <param name="navigationPage">The navigation page receiving the back request.</param>
    /// <returns>A task completing after the back request is handled.</returns>
    /// <remarks>A pending modal confirmation prevents a duplicate request. An attempt to pop the application's main page quits the application.</remarks>
    public static async Task HandleNavigationPageGoBack(NavigationPage navigationPage)
    {
        if (navigationPage.GetParentWindow() is PrismWindow window &&
            window.PendingModalConfirmation is Page pendingModal &&
            (pendingModal == navigationPage || PageNavigationHistory.IsDescendantOf(navigationPage, pendingModal)))
        {
            return;
        }

        var navigationService = Navigation.Xaml.Navigation.GetNavigationService(navigationPage.CurrentPage);
        var result = await navigationService.GoBackAsync();
        if (result.Exception is NavigationException { Message: NavigationException.CannotPopApplicationMainPage })
        {
            Application.Current?.Quit();
        }
    }

    /// <summary>Routes dismissal of a navigation page through a modal back navigation request.</summary>
    /// <param name="navigationPage">The dismissed navigation page.</param>
    /// <returns>A task completing after the request is handled.</returns>
    public static async Task HandleNavigationPageSwipedAway(NavigationPage navigationPage)
    {
        var navigationService = Navigation.Xaml.Navigation.GetNavigationService(navigationPage.CurrentPage);
        var navParams = new NavigationParameters()
        {
            {
                KnownNavigationParameters.UseModalNavigation, true
            },
        };
        var result = await navigationService.GoBackAsync(navParams);
        if (result.Exception is NavigationException { Message: NavigationException.CannotPopApplicationMainPage })
        {
            Application.Current?.Quit();
        }
    }

    /// <summary>Notifies navigation callbacks and destroys the departed page after a platform pop.</summary>
    /// <param name="previousPage">The page removed by platform navigation.</param>
    /// <param name="currentPage">The view displayed after the pop.</param>
    public static void HandleSystemGoBack(IView? previousPage, IView? currentPage)
    {
        var parameters = new NavigationParameters();
        parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
        OnNavigatedFrom(previousPage, parameters);
        OnNavigatedTo(GetOnNavigatedToTargetFromChild(currentPage), parameters);
        DestroyPage(previousPage);
    }

    internal static bool HasDirectNavigationPageParent(Page page)
    {
        return page.Parent is NavigationPage;
    }

    internal static bool HasNavigationPageParent(Page page) =>
        HasNavigationPageParent(page, out var _);

    internal static bool HasNavigationPageParent(Page page, out NavigationPage? navigationPage)
    {
        if (page.Parent != null)
        {
            if (page.Parent is NavigationPage navParent)
            {
                navigationPage = navParent;
                return true;
            }
            else if (page.Parent is TabbedPage && page.Parent?.Parent is NavigationPage navigationParent)
            {
                navigationPage = navigationParent;
                return true;
            }
        }

        navigationPage = null;
        return false;
    }

    internal static bool IsSameOrSubclassOf<T>(Type potentialDescendant)
    {
        if (potentialDescendant == null)
            return false;

        Type potentialBase = typeof(T);

        return potentialDescendant.GetTypeInfo().IsSubclassOf(potentialBase)
               || potentialDescendant == potentialBase;
    }
}
