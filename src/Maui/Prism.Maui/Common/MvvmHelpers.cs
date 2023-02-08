using System.ComponentModel;
using System.Reflection;
using Prism.Navigation;
using Prism.Navigation.Xaml;
using Prism.Regions.Navigation;
using NavigationMode = Prism.Navigation.NavigationMode;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Prism.Common;

public static class MvvmHelpers
{
    public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
    {
        if (view is T viewAsT)
        {
            action(viewAsT);
        }

        if (view is BindableObject element && element.BindingContext is T viewModelAsT)
        {
            action(viewModelAsT);
        }

        if(view is Page page)
        {
            var children = page.GetChildRegions();
            if(children is not null)
                foreach (var child in children)
                    InvokeViewAndViewModelAction<T>(child, action);
        }
    }

    public static async Task InvokeViewAndViewModelActionAsync<T>(object view, Func<T, Task> action) where T : class
    {
        if (view is T viewAsT)
        {
            await action(viewAsT);
        }

        if (view is BindableObject element && element.BindingContext is T viewModelAsT)
        {
            await action(viewModelAsT);
        }

        if (view is Page page)
        {
            var children = page.GetChildRegions();
            if(children is not null)
                foreach (var child in children)
                    await InvokeViewAndViewModelActionAsync<T>(child, action);
        }
    }

    public static void DestroyPage(IView view)
    {
        try
        {
            DestroyChildren(view);

            InvokeViewAndViewModelAction<IDestructible>(view, v => v.Destroy());

            if(view is Page page)
            {
                page.Behaviors?.Clear();
                page.BindingContext = null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot destroy {view}.", ex);
        }
    }

    private static void DestroyChildren(IView page)
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

    public static void DestroyWithModalStack(Page page, IList<Page> modalStack)
    {
        foreach (var childPage in modalStack.Reverse())
        {
            DestroyPage(childPage);
        }
        DestroyPage(page);
    }

    public static T GetImplementerFromViewOrViewModel<T>(object view)
            where T : class
    {
        if (view is T viewAsT)
        {
            return viewAsT;
        }

        if (view is VisualElement element && element.BindingContext is T vmAsT)
        {
            return vmAsT;
        }

        return null;
    }

    public static bool IsNavigationTarget(object view, INavigationContext navigationContext)
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

    public static void OnNavigatedFrom(object view, INavigationContext navigationContext)
    {
        InvokeViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedFrom(navigationContext));
    }

    public static void OnNavigatedTo(object view, INavigationContext navigationContext)
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

    private static bool CanNavigate(object page, INavigationParameters parameters)
    {
        var implementer = GetImplementerFromViewOrViewModel<IConfirmNavigation>(page);
        return implementer?.CanNavigate(parameters) ?? true;
    }

    public static void OnNavigatedFrom(object page, INavigationParameters parameters)
    {
        if (page != null)
            InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedFrom(parameters));
    }

    public static async Task OnInitializedAsync(object page, INavigationParameters parameters)
    {
        if (page is null) return;

        InvokeViewAndViewModelAction<IInitialize>(page, v => v.Initialize(parameters));
        await InvokeViewAndViewModelActionAsync<IInitializeAsync>(page, async v => await v.InitializeAsync(parameters));
    }

    private static bool HasKey(this IEnumerable<KeyValuePair<string, object>> parameters, string name, out string key)
    {
        key = parameters.Select(x => x.Key).FirstOrDefault(k => k.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        return !string.IsNullOrEmpty(key);
    }

    public static void OnNavigatedTo(object page, INavigationParameters parameters)
    {
        if (page != null)
            InvokeViewAndViewModelAction<INavigatedAware>(page, v => v.OnNavigatedTo(parameters));
    }

    public static Page GetOnNavigatedToTarget(Page page, IView mainPage, bool useModalNavigation)
    {
        Page target;
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

    public static Page GetOnNavigatedToTargetFromChild(IView target)
    {
        Page child = null;

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

    public static Page GetPreviousPage(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
    {
        Page previousPage = null;

        int currentPageIndex = GetCurrentPageIndex(currentPage, navStack);
        int previousPageIndex = currentPageIndex - 1;
        if (navStack.Count >= 0 && previousPageIndex >= 0)
            previousPage = navStack[previousPageIndex];

        return previousPage;
    }

    public static int GetCurrentPageIndex(Page currentPage, System.Collections.Generic.IReadOnlyList<Page> navStack)
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

    public static Page GetCurrentPage(Page mainPage) =>
        _getCurrentPage(mainPage);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetCurrentPageDelegate(Func<Page, Page> getCurrentPageDelegate) =>
        _getCurrentPage = getCurrentPageDelegate;

    private static Func<Page, Page> _getCurrentPage = mainPage =>
    {
        var page = mainPage;

        var lastModal = page.Navigation.ModalStack.LastOrDefault();
        if (lastModal != null)
            page = lastModal;

        return GetOnNavigatedToTargetFromChild(page);
    };

    public static async Task HandleNavigationPageGoBack(NavigationPage navigationPage)
    {
        var navigationService = Navigation.Xaml.Navigation.GetNavigationService(navigationPage.CurrentPage);
        await navigationService.GoBackAsync();
    }

    public static void HandleSystemGoBack(IView previousPage, IView currentPage)
    {
        var parameters = new NavigationParameters();
        parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);
        OnNavigatedFrom(previousPage, parameters);
        OnNavigatedTo(GetOnNavigatedToTargetFromChild(currentPage), parameters);
        DestroyPage(previousPage);
    }

    internal static bool HasDirectNavigationPageParent(Page page)
    {
        return page?.Parent != null && page?.Parent is NavigationPage;
    }

    internal static bool HasNavigationPageParent(Page page) =>
        HasNavigationPageParent(page, out var _);

    internal static bool HasNavigationPageParent(Page page, out NavigationPage navigationPage)
    {
        if (page?.Parent != null)
        {
            if (page.Parent is NavigationPage navParent)
            {
                navigationPage = navParent;
                return true;
            }
            else if ((page.Parent is TabbedPage) && page.Parent?.Parent is NavigationPage navigationParent)
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
