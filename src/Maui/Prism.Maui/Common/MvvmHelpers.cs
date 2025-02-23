#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Prism.Dialogs;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Xaml;
using NavigationMode = Prism.Navigation.NavigationMode;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.Common;

public static class MvvmHelpers
{
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

    public static void DestroyPage(IView? view)
    {
        try
        {
            DestroyChildren(view);

            InvokeViewAndViewModelAction<IDestructible>(view, v => v.Destroy());

            if (view is VisualElement visualElement)
            {
#if ANDROID
                visualElement.Unloaded += VisualElementUnloaded;
                void VisualElementUnloaded(object? sender, EventArgs e)
                {
                    visualElement.Unloaded -= VisualElementUnloaded;
                    visualElement.Behaviors?.Clear();
                    visualElement.BindingContext = null;
                }
#else
                visualElement.Behaviors?.Clear();
                visualElement.BindingContext = null;
#endif
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot destroy {view}.", ex);
        }
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

    public static void DestroyWithModalStack(Page? page, IList<Page?> modalStack)
    {
        foreach (var childPage in modalStack.Reverse())
        {
            DestroyPage(childPage);
        }

        DestroyPage(page);
    }

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

    public static void OnNavigatedFrom(object? view, NavigationContext navigationContext)
    {
        InvokeViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedFrom(navigationContext));
    }

    public static void OnNavigatedTo(object? view, NavigationContext navigationContext)
    {
        InvokeViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedTo(navigationContext));
    }

    public static Task<bool> CanNavigateAsync(object page, INavigationParameters parameters)
    {
        var implementer = GetImplementerFromViewOrViewModel<IConfirmNavigationAsync>(page);
        if (implementer is null)
            return Task.FromResult(CanNavigate(page, parameters));

        return implementer.CanNavigateAsync(parameters);
    }

    public static bool CanNavigate(object page, INavigationParameters parameters)
    {
        var implementer = GetImplementerFromViewOrViewModel<IConfirmNavigation>(page);
        return implementer?.CanNavigate(parameters) ?? true;
    }

    public static void OnNavigatedFrom(object? page, INavigationParameters parameters)
    {
        if (page != null)
            InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
    }

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

    public static void OnNavigatedTo(object? page, INavigationParameters parameters)
    {
        if (page != null)
            InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
    }

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

    public static Page? GetPreviousPage(Page currentPage, IReadOnlyList<Page?> navStack)
    {
        Page? previousPage = null;

        int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
        int previousPageIndex = currentPageIndex - 1;
        if (navStack.Count >= 0 && previousPageIndex >= 0)
            previousPage = navStack[previousPageIndex];

        return previousPage;
    }

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

    public static Page? GetCurrentPage(Page mainPage) =>
        _getCurrentPage(mainPage);

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

    public static async Task HandleNavigationPageGoBack(NavigationPage navigationPage)
    {
        var navigationService = Navigation.Xaml.Navigation.GetNavigationService(navigationPage.CurrentPage);
        var result = await navigationService.GoBackAsync();
        if (result.Exception is NavigationException { Message: NavigationException.CannotPopApplicationMainPage })
        {
            Application.Current?.Quit();
        }
    }

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

    internal static async Task<Page?> FindPageByNameAsync(Page? current, string name)
    {
        if (current == null) return null;

        Page? navPage = current.Navigation.NavigationStack.LastOrDefault();
        if(navPage == null)
        {
            navPage = current.Parent switch
            {
                Page parentPage => parentPage,
                Window window => window.Page,
                _ => null,
            };
        }

        var modal = current.Navigation.ModalStack.LastOrDefault();
        while (modal != null)
        {
            var target = await FindPageRecursivelyAsync(modal, name);
            if (target != null) return target;

            modal = GetPreviousPage(modal, modal.Navigation.ModalStack);
            if(modal != null) await modal.Navigation.PopModalAsync();
        }

        return await FindPageRecursivelyAsync(navPage, name);
    }

    private static async Task<Page?> FindPageRecursivelyAsync(Page? target, string name)
    {
        if (target != null && ViewModelLocator.GetNavigationName(target) == name) return target;

        switch (target)
        {
            case NavigationPage navigation:
                var page = navigation.Navigation.NavigationStack.LastOrDefault();
                while (page != null)
                {
                    if (ViewModelLocator.GetNavigationName(page) == name) return page;
                    if (navigation.Navigation.NavigationStack.Count == 1) break;

                    await navigation.Navigation.PopAsync();
                    page = navigation.Navigation.NavigationStack.LastOrDefault();
                }
                return navigation.Parent switch
                {
                    Page parentPage => await FindPageRecursivelyAsync(parentPage, name),
                    _ => null,
                };
            case TabbedPage:
            case FlyoutPage:
            case ContentPage:
                return target.Parent switch
                {
                    Page parentPage => await FindPageRecursivelyAsync(parentPage, name),
                    _ => null,
                };
        }

        return null;
    }
}
