using System.Text.RegularExpressions;
using System.Web;
using Prism.Common;
using Prism.Events;
using Prism.Extensions;
using Prism.Mvvm;
using Application = Microsoft.Maui.Controls.Application;
using XamlTab = Prism.Navigation.Xaml.TabbedPage;

namespace Prism.Navigation;

/// <summary>
/// Provides page based navigation for ViewModels.
/// </summary>
public class PageNavigationService : INavigationService, IRegistryAware
{
    private static readonly SemaphoreSlim _semaphore = new (1, 1);
    private static DateTime _lastNavigate;
    internal const string RemovePageRelativePath = "../";
    internal const string RemovePageInstruction = "__RemovePage/";
    internal const string RemovePageSegment = "__RemovePage";

    internal static PageNavigationSource NavigationSource { get; set; } = PageNavigationSource.Device;

    private readonly IContainerProvider _container;
    protected readonly IWindowManager _windowManager;
    protected readonly IPageAccessor _pageAccessor;
    protected readonly IEventAggregator _eventAggregator;

    private Window _window;
    protected Window Window
    {
        get
        {
            if(_window is null && _pageAccessor.Page is not null)
            {
                _window = _pageAccessor.Page.GetParentWindow();
            }
            else
            {
                _window = _windowManager.Windows.OfType<PrismWindow>().FirstOrDefault();
            }

            return _window;
        }
    }

    // This should be resolved by the container when accessed as a Module could register views after the NavigationService was resolved
    public IViewRegistry Registry => _container.Resolve<INavigationRegistry>();

    /// <summary>
    /// Constructs a new instance of the <see cref="PageNavigationService"/>.
    /// </summary>
    /// <param name="container">The <see cref="IContainerProvider"/> that will be used to resolve pages for navigation.</param>
    /// <param name="windowManager">The <see cref="IWindowManager"/> that will let the NavigationService retrieve, open or close the app Windows.</param>
    /// <param name="eventAggregator">The <see cref="IEventAggregator"/> that will raise <see cref="NavigationRequestEvent"/>.</param>
    /// <param name="pageAccessor">The <see cref="IPageAccessor"/> that will let the NavigationService retrieve the <see cref="Page"/> for the current scope.</param>q
    public PageNavigationService(IContainerProvider container,
        IWindowManager windowManager,
        IEventAggregator eventAggregator,
        IPageAccessor pageAccessor)
    {
        _container = container;
        _windowManager = windowManager;
        _eventAggregator = eventAggregator;
        _pageAccessor = pageAccessor;
    }

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping the calling Page off the navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns>If <c>true</c> a go back operation was successful. If <c>false</c> the go back operation failed.</returns>
    public virtual async Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
    {
        await _semaphore.WaitAsync();

        INavigationResult result = await GoBackInternalAsync(parameters);

        _semaphore.Release();

        return result;
    }

    private async Task<INavigationResult> GoBackInternalAsync(INavigationParameters parameters)
    {
        Page page = null;
        try
        {
            parameters ??= new NavigationParameters();

            NavigationSource = PageNavigationSource.NavigationService;

            page = GetCurrentPage();
            if (IsRoot(GetPageFromWindow(), page))
                throw new NavigationException(NavigationException.CannotPopApplicationMainPage, page);

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            var canNavigate = await MvvmHelpers.CanNavigateAsync(page, parameters);
            if (!canNavigate)
            {
                throw new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, page);
            }

            bool useModalForDoPop = UseModalGoBack(page, parameters);
            Page previousPage = MvvmHelpers.GetOnNavigatedToTarget(page, Window?.Page, useModalForDoPop);

            bool? animated = parameters.ContainsKey(KnownNavigationParameters.Animated) ? parameters.GetValue<bool>(KnownNavigationParameters.Animated) : null;
            var poppedPage = await DoPop(page.Navigation, useModalForDoPop, animated ?? true);
            if (poppedPage != null)
            {
                MvvmHelpers.OnNavigatedFrom(page, parameters);
                MvvmHelpers.OnNavigatedTo(previousPage, parameters);
                MvvmHelpers.DestroyPage(poppedPage);

                return Notify(NavigationRequestType.GoBack, parameters);
            }
        }
        catch (Exception ex)
        {
            return Notify(NavigationRequestType.GoBack, parameters, ex);
        }
        finally
        {
            NavigationSource = PageNavigationSource.Device;
        }

        return Notify(NavigationRequestType.GoBack, parameters, GetGoBackException(page, GetPageFromWindow()));
    }

    /// <inheritdoc />
    public virtual async Task<INavigationResult> GoBackAsync(string viewName, INavigationParameters parameters)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (parameters is null)
                parameters = new NavigationParameters();

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            var page = GetCurrentPage();
            var canNavigate = await MvvmHelpers.CanNavigateAsync(page, parameters);
            if (!canNavigate)
            {
                throw new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, page);
            }

            var pagesToDestroy = page.Navigation.NavigationStack.ToList(); // get all pages to destroy
            pagesToDestroy.Reverse(); // destroy them in reverse order
            var goBackPage = pagesToDestroy.FirstOrDefault(p => ViewModelLocator.GetNavigationName(p) == viewName); // find the go back page
            if (goBackPage is null)
            {
                throw new NavigationException(NavigationException.GoBackRequiresNavigationPage);
            }
            var index = pagesToDestroy.IndexOf(goBackPage);
            pagesToDestroy.RemoveRange(index, pagesToDestroy.Count - index); // don't destroy pages from the go back page to the root page
            var pagesToRemove = pagesToDestroy.Skip(1).ToList(); // exclude the current page from the destroy pages

            bool animated = parameters.ContainsKey(KnownNavigationParameters.Animated) ? parameters.GetValue<bool>(KnownNavigationParameters.Animated) : true;
            NavigationSource = PageNavigationSource.NavigationService;
            foreach(var removePage in pagesToRemove)
            {
                page.Navigation.RemovePage(removePage);
            }
            await page.Navigation.PopAsync(animated);
            NavigationSource = PageNavigationSource.Device;

            foreach (var destroyPage in pagesToDestroy)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            MvvmHelpers.OnNavigatedTo(goBackPage, parameters);

            return Notify(NavigationRequestType.GoBack, parameters);
        }
        catch (Exception ex)
        {
            return Notify(NavigationRequestType.GoBack, parameters, ex);
        }
        finally
        {
            NavigationSource = PageNavigationSource.Device;
            _semaphore.Release();
        }
    }


    private static Exception GetGoBackException(Page currentPage, IView mainPage)
    {
        if (IsMainPage(currentPage, mainPage))
        {
            return new NavigationException(NavigationException.CannotPopApplicationMainPage, currentPage);
        }
        else if ((currentPage is NavigationPage navPage && IsOnNavigationPageRoot(navPage)) ||
            (currentPage.Parent is NavigationPage navParent && IsOnNavigationPageRoot(navParent)))
        {
            return new NavigationException(NavigationException.CannotGoBackFromRoot, currentPage);
        }

        return new NavigationException(NavigationException.UnknownException, currentPage);
    }

    private static bool IsOnNavigationPageRoot(NavigationPage navigationPage) =>
        navigationPage.CurrentPage == navigationPage.RootPage;

    private static bool IsMainPage(IView currentPage, IView mainPage)
    {
        if (currentPage == mainPage)
        {
            return true;
        }
        else if (mainPage is FlyoutPage flyout && flyout.Detail == currentPage)
        {
            return true;
        }
        else if (currentPage.Parent is TabbedPage tabbed && mainPage == tabbed)
        {
            return true;
        }
        else if (currentPage.Parent is NavigationPage navPage && navPage.CurrentPage == navPage.RootPage)
        {
            return IsMainPage(navPage, mainPage);
        }

        return false;
    }

    /// <summary>
    /// When navigating inside a NavigationPage: Pops all but the root Page off the navigation stack
    /// </summary>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    /// <remarks>Only works when called from a View within a NavigationPage</remarks>
    public virtual async Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (parameters is null)
                parameters = new NavigationParameters();

            parameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.Back);

            var page = GetCurrentPage();
            var canNavigate = await MvvmHelpers.CanNavigateAsync(page, parameters);
            if (!canNavigate)
            {
                throw new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, page);
            }

            var pagesToDestroy = page.Navigation.NavigationStack.ToList(); // get all pages to destroy
            pagesToDestroy.Reverse(); // destroy them in reverse order
            var root = pagesToDestroy.Last();
            pagesToDestroy.Remove(root); //don't destroy the root page

            bool animated = parameters.ContainsKey(KnownNavigationParameters.Animated) ? parameters.GetValue<bool>(KnownNavigationParameters.Animated) : true;
            NavigationSource = PageNavigationSource.NavigationService;
            await page.Navigation.PopToRootAsync(animated);
            NavigationSource = PageNavigationSource.Device;

            foreach (var destroyPage in pagesToDestroy)
            {
                MvvmHelpers.OnNavigatedFrom(destroyPage, parameters);
                MvvmHelpers.DestroyPage(destroyPage);
            }

            MvvmHelpers.OnNavigatedTo(root, parameters);

            return Notify(NavigationRequestType.GoToRoot, parameters);
        }
        catch (Exception ex)
        {
            return Notify(NavigationRequestType.GoToRoot, parameters, ex);
        }
        finally
        {
            NavigationSource = PageNavigationSource.Device;
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The Uri to navigate to</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <remarks>Navigation parameters can be provided in the Uri and by using the <paramref name="parameters"/>.</remarks>
    /// <example>
    /// NavigateAsync(new Uri("MainPage?id=3&amp;name=dan", UriKind.RelativeSource), parameters);
    /// </example>
    public virtual async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
    {
        await _semaphore.WaitAsync();
        // Ensure adequate time has passed since last navigation so that UI Refresh can Occur
        if (DateTime.Now - _lastNavigate < TimeSpan.FromMilliseconds(150))
        {
            await Task.Delay(150);
        }

        try
        {
            parameters ??= new NavigationParameters();

            NavigationSource = PageNavigationSource.NavigationService;

            var navigationSegments = UriParsingHelper.GetUriSegments(uri);

            if (uri.IsAbsoluteUri)
            {
                await ProcessNavigationForAbsoluteUri(navigationSegments, parameters, null, null);
            }
            else
            {
                await ProcessNavigation(GetCurrentPage(), navigationSegments, parameters, null, null);
            }

            return Notify(uri, parameters);
        }
        catch (Exception ex)
        {
            return Notify(uri, parameters, ex);
        }
        finally
        {
            _lastNavigate = DateTime.Now;
            NavigationSource = PageNavigationSource.Device;
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Selects a Tab of the TabbedPage parent.
    /// </summary>
    /// <param name="tabName">The name of the tab to select</param>
    /// <param name="parameters">The navigation parameters</param>
    /// <returns><see cref="INavigationResult"/> indicating whether the request was successful or if there was an encountered <see cref="Exception"/>.</returns>
    public virtual async Task<INavigationResult> SelectTabAsync(string tabName, INavigationParameters parameters)
    {
        try
        {
            var tabbedPage = GetTabbedPage(_pageAccessor.Page);
            TabbedPage GetTabbedPage(Element page) =>
                page switch
                {
                    TabbedPage tabbedPage => tabbedPage,
                    null => null,
                    _ => GetTabbedPage(page.Parent)
                };

            if (tabbedPage is null)
                throw new NullReferenceException("The Page is null.");

            var parts = tabName.Split('|');
            Page selectedChild = null;
            if (parts.Length == 1)
                selectedChild = tabbedPage.Children.FirstOrDefault(x => ViewModelLocator.GetNavigationName(x) == tabName || (x is NavigationPage navPage && ViewModelLocator.GetNavigationName(navPage.RootPage) == tabName));
            else if (parts.Length == 2)
                selectedChild = tabbedPage.Children.FirstOrDefault(x => x is NavigationPage navPage && ViewModelLocator.GetNavigationName(navPage) == parts[0] && ViewModelLocator.GetNavigationName(navPage.RootPage) == parts[1]);
            else
                throw new NavigationException($"Invalid Tab Name: {tabName}");

            if (selectedChild is null)
                throw new NavigationException($"No Tab found with the Name: {tabName}");

            var navigatedFromPage = _pageAccessor.Page;
            if (!await MvvmHelpers.CanNavigateAsync(navigatedFromPage, parameters))
                throw new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, navigatedFromPage);

            tabbedPage.CurrentPage = selectedChild;
            MvvmHelpers.OnNavigatedFrom(navigatedFromPage, parameters);
            MvvmHelpers.OnNavigatedTo(selectedChild, parameters);

            return new NavigationResult();
        }
        catch (Exception ex)
        {
            return new NavigationResult(ex);
        }
    }

    /// <summary>
    /// Processes the Navigation for the Queued navigation segments
    /// </summary>
    /// <param name="currentPage">The Current <see cref="Page"/> that we are navigating from.</param>
    /// <param name="segments">The Navigation <see cref="Uri"/> segments.</param>
    /// <param name="parameters">The <see cref="INavigationParameters"/>.</param>
    /// <param name="useModalNavigation"><see cref="Nullable{Boolean}"/> flag if we should force Modal Navigation.</param>
    /// <param name="animated">If <c>true</c>, the navigation will be animated.</param>
    /// <returns></returns>
    protected virtual async Task ProcessNavigation(Page currentPage, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        if (segments.Count == 0)
        {
            return;
        }

        var nextSegment = segments.Dequeue();

        var pageParameters = UriParsingHelper.GetSegmentParameters(nextSegment, parameters);

        if (pageParameters.TryGetValue<bool>(KnownNavigationParameters.UseModalNavigation, out var parameterModal))
            useModalNavigation = parameterModal;

        bool? pageAnimation = animated;
        if (animated is null && pageParameters.TryGetValue<bool>(KnownNavigationParameters.Animated, out var parameterAnimation))
        {
            pageAnimation = parameterAnimation;
        }

        if (nextSegment == RemovePageSegment)
        {
            await ProcessNavigationForRemovePageSegments(currentPage, nextSegment, segments, parameters, useModalNavigation, pageAnimation);
            return;
        }

        if (currentPage is null)
        {
            await ProcessNavigationForRootPage(nextSegment, segments, parameters, useModalNavigation, pageAnimation);
            return;
        }

        if (currentPage is ContentPage)
        {
            await ProcessNavigationForContentPage(currentPage, nextSegment, segments, parameters, useModalNavigation, pageAnimation);
        }
        else if (currentPage is NavigationPage nav)
        {
            await ProcessNavigationForNavigationPage(nav, nextSegment, segments, parameters, useModalNavigation, pageAnimation);
        }
        else if (currentPage is TabbedPage tabbed)
        {
            await ProcessNavigationForTabbedPage(tabbed, nextSegment, segments, parameters, useModalNavigation, pageAnimation);
        }
        else if (currentPage is FlyoutPage flyout)
        {
            await ProcessNavigationForFlyoutPage(flyout, nextSegment, segments, parameters, useModalNavigation, pageAnimation);
        }
    }

    protected virtual Task ProcessNavigationForRemovePageSegments(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        if (!MvvmHelpers.HasDirectNavigationPageParent(currentPage))
        {
            throw new NavigationException(NavigationException.RelativeNavigationRequiresNavigationPage, currentPage);
        }

        return CanRemoveAndPush(segments)
            ? RemoveAndPush(currentPage, nextSegment, segments, parameters, useModalNavigation, animated)
            : RemoveAndGoBack(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
    }

    private static bool CanRemoveAndPush(Queue<string> segments)
    {
        return !segments.All(x => x == RemovePageSegment);
    }

    private Task RemoveAndGoBack(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        var pagesToRemove = new List<Page>();

        var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
        if (currentPage.Navigation.NavigationStack.Count > 0)
        {
            currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;
        }

        while (segments.Count != 0)
        {
            currentPageIndex -= 1;
            pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
            nextSegment = segments.Dequeue();
        }

        RemovePagesFromNavigationPage(currentPage, pagesToRemove);

        return GoBackInternalAsync(parameters);
    }

    private async Task RemoveAndPush(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        var pagesToRemove = new List<Page>
        {
            currentPage
        };

        var currentPageIndex = currentPage.Navigation.NavigationStack.Count;
        if (currentPage.Navigation.NavigationStack.Count > 0)
        {
            currentPageIndex = currentPage.Navigation.NavigationStack.Count - 1;
        }

        while (segments.Peek() == RemovePageSegment)
        {
            currentPageIndex -= 1;
            pagesToRemove.Add(currentPage.Navigation.NavigationStack[currentPageIndex]);
            nextSegment = segments.Dequeue();
        }

        await ProcessNavigation(currentPage, segments, parameters, useModalNavigation, animated);

        RemovePagesFromNavigationPage(currentPage, pagesToRemove);
    }

    private static void RemovePagesFromNavigationPage(Page currentPage, List<Page> pagesToRemove)
    {
        var navigationPage = (NavigationPage)currentPage.Parent;
        foreach (var page in pagesToRemove)
        {
            navigationPage.Navigation.RemovePage(page);
            MvvmHelpers.DestroyPage(page);
        }
    }

    protected virtual Task ProcessNavigationForAbsoluteUri(Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        return ProcessNavigation(null, segments, parameters, useModalNavigation, animated);
    }

    protected virtual async Task ProcessNavigationForRootPage(string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        var nextPage = CreatePageFromSegment(nextSegment);
        if (nextPage is TabbedPage tabbedPage)
            await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);

        await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

        var currentPage = GetPageFromWindow();
        var modalStack = currentPage?.Navigation.ModalStack.ToList();
        await DoNavigateAction(GetCurrentPage(), nextSegment, nextPage, parameters, async () =>
        {
            await DoPush(null, nextPage, useModalNavigation, animated);
        });
        if (currentPage != null)
        {
            MvvmHelpers.DestroyWithModalStack(currentPage, modalStack);
        }
    }

    protected virtual async Task ProcessNavigationForContentPage(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        var nextPageType = Registry.GetViewType(UriParsingHelper.GetSegmentName(nextSegment));
        bool useReverse = UseReverseNavigation(currentPage, nextPageType) && !(useModalNavigation.HasValue && useModalNavigation.Value);
        if (!useReverse)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            if (nextPage is TabbedPage tabbedPage)
                await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);

            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);

            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated);
            });
        }
        else
        {
            await UseReverseNavigation(currentPage, nextSegment, segments, parameters, useModalNavigation, animated);
        }
    }

    protected virtual async Task ProcessNavigationForNavigationPage(NavigationPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        if (currentPage.Navigation.NavigationStack.Count == 0)
        {
            await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);
            return;
        }

        var clearNavigationStack = GetClearNavigationPageNavigationStack(currentPage);
        var isEmptyOfNavigationStack = currentPage.Navigation.NavigationStack.Count == 0;

        List<Page> destroyPages;
        if (clearNavigationStack && !isEmptyOfNavigationStack)
        {
            destroyPages = currentPage.Navigation.NavigationStack.ToList();
            destroyPages.Reverse();

            await currentPage.Navigation.PopToRootAsync(false);
        }
        else
        {
            destroyPages = new List<Page>();
        }

        var topPage = currentPage.Navigation.NavigationStack.LastOrDefault();
        var nextPageType = Registry.GetViewType(UriParsingHelper.GetSegmentName(nextSegment));
        if (topPage?.GetType() == nextPageType)
        {
            if (clearNavigationStack)
                destroyPages.Remove(destroyPages.Last());

            if (segments.Count > 0)
                await UseReverseNavigation(topPage, segments.Dequeue(), segments, parameters, false, animated);

            await DoNavigateAction(topPage, nextSegment, topPage, parameters, onNavigationActionCompleted: (p) =>
            {
                if (nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                {
                    var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment, parameters);
                    SelectPageTab(topPage, segmentParams);
                }
            });
        }
        else
        {
            await UseReverseNavigation(currentPage, nextSegment, segments, parameters, false, animated);

            if (clearNavigationStack && !isEmptyOfNavigationStack)
            {
                currentPage.Navigation.RemovePage(topPage);
            }
        }

        foreach (var destroyPage in destroyPages)
        {
            MvvmHelpers.DestroyPage(destroyPage);
        }
    }

    protected virtual async Task ProcessNavigationForTabbedPage(TabbedPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        var nextPage = CreatePageFromSegment(nextSegment);
        if (nextPage is TabbedPage tabbedPage)
            await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);
        await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
        await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
        {
            await DoPush(currentPage, nextPage, useModalNavigation, animated);
        });
    }

    protected virtual async Task ProcessNavigationForFlyoutPage(FlyoutPage currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        bool isPresented = GetFlyoutPageIsPresented(currentPage);

        var detail = currentPage.Detail;
        if (detail is null)
        {
            var newDetail = CreatePageFromSegment(nextSegment);
            if (newDetail is TabbedPage tabbedPage)
                await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);
            await ProcessNavigation(newDetail, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(null, nextSegment, newDetail, parameters, onNavigationActionCompleted: (p) =>
            {
                currentPage.IsPresented = isPresented;
                currentPage.Detail = newDetail;
            });
            return;
        }

        if (useModalNavigation.HasValue && useModalNavigation.Value)
        {
            var nextPage = CreatePageFromSegment(nextSegment);
            if (nextPage is TabbedPage tabbedPage)
                await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);
            await ProcessNavigation(nextPage, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(currentPage, nextSegment, nextPage, parameters, async () =>
            {
                currentPage.IsPresented = isPresented;
                await DoPush(currentPage, nextPage, true, animated);
            });
            return;
        }

        var nextSegmentType = Registry.GetViewType(UriParsingHelper.GetSegmentName(nextSegment));

        //we must recreate the NavigationPage every time or the transitions on iOS will not work properly, unless we meet the two scenarios below
        bool detailIsNavPage = false;
        bool reuseNavPage = false;
        if (detail is NavigationPage navPage)
        {
            detailIsNavPage = true;

            //we only care if we the next segment is also a NavigationPage.
            if (MvvmHelpers.IsSameOrSubclassOf<NavigationPage>(nextSegmentType))
            {
                //first we check to see if we are being forced to reuse the NavPage by checking the interface
                reuseNavPage = !GetClearNavigationPageNavigationStack(navPage);

                if (!reuseNavPage)
                {
                    //if we weren't forced to reuse the NavPage, then let's check the NavPage.CurrentPage against the next segment type as we don't want to recreate the entire nav stack
                    //just in case the user is trying to navigate to the same page which may be nested in a NavPage
                    var nextPageType = Registry.GetViewType(UriParsingHelper.GetSegmentName(segments.Peek()));
                    var currentPageType = navPage.CurrentPage.GetType();
                    if (nextPageType == currentPageType)
                    {
                        reuseNavPage = true;
                    }
                }
            }
        }

        if ((detailIsNavPage && reuseNavPage) || (!detailIsNavPage && detail.GetType() == nextSegmentType))
        {
            await ProcessNavigation(detail, segments, parameters, useModalNavigation, animated);
            await DoNavigateAction(null, nextSegment, detail, parameters, onNavigationActionCompleted: (p) =>
            {
                if (detail is TabbedPage && nextSegment.Contains(KnownNavigationParameters.SelectedTab))
                {
                    var segmentParams = UriParsingHelper.GetSegmentParameters(nextSegment, parameters);
                    SelectPageTab(detail, segmentParams);
                }

                currentPage.IsPresented = isPresented;
            });
            return;
        }
        else
        {
            var newDetail = CreatePageFromSegment(nextSegment);
            if (newDetail is TabbedPage tabbedPage)
                await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);
            await ProcessNavigation(newDetail, segments, parameters, newDetail is NavigationPage ? false : true, animated);
            await DoNavigateAction(detail, nextSegment, newDetail, parameters, onNavigationActionCompleted: (p) =>
            {
                if (detailIsNavPage)
                    OnNavigatedFrom(((NavigationPage)detail).CurrentPage, p);

                currentPage.IsPresented = isPresented;
                currentPage.Detail = newDetail;
                MvvmHelpers.DestroyPage(detail);
            });
            return;
        }
    }

    protected static bool GetFlyoutPageIsPresented(FlyoutPage page)
    {
        if (page is IFlyoutPageOptions flyoutPageOptions)
            return flyoutPageOptions.IsPresentedAfterNavigation;

        else if (page.BindingContext is IFlyoutPageOptions flyoutPageBindingContext)
            return flyoutPageBindingContext.IsPresentedAfterNavigation;

        return false;
    }

    protected static bool GetClearNavigationPageNavigationStack(NavigationPage page)
    {
        if (page is INavigationPageOptions iNavigationPage)
            return iNavigationPage.ClearNavigationStackOnNavigation;

        else if (page.BindingContext is INavigationPageOptions iNavigationPageBindingContext)
            return iNavigationPageBindingContext.ClearNavigationStackOnNavigation;

        return true;
    }

    protected static async Task DoNavigateAction(Page fromPage, string toSegment, Page toPage, INavigationParameters parameters, Func<Task> navigationAction = null, Action<INavigationParameters> onNavigationActionCompleted = null)
    {
        var segmentParameters = UriParsingHelper.GetSegmentParameters(toSegment, parameters);
        segmentParameters.GetNavigationParametersInternal().Add(KnownInternalParameters.NavigationMode, NavigationMode.New);

        var canNavigate = await MvvmHelpers.CanNavigateAsync(fromPage, segmentParameters);
        if (!canNavigate)
        {
            throw new NavigationException(NavigationException.IConfirmNavigationReturnedFalse, toPage);
        }

        await OnInitializedAsync(toPage, segmentParameters);

        if (navigationAction != null)
        {
            await navigationAction();
        }

        OnNavigatedFrom(fromPage, segmentParameters);

        onNavigationActionCompleted?.Invoke(segmentParameters);

        OnNavigatedTo(toPage, segmentParameters);
    }

    static async Task OnInitializedAsync(Page toPage, INavigationParameters parameters)
    {
        await MvvmHelpers.OnInitializedAsync(toPage, parameters);

        if (toPage is TabbedPage tabbedPage)
        {
            foreach (var child in tabbedPage.Children)
            {
                if (child is NavigationPage navigationPage)
                {
                    await MvvmHelpers.OnInitializedAsync(navigationPage.CurrentPage, parameters);
                }
                else
                {
                    await MvvmHelpers.OnInitializedAsync(child, parameters);
                }
            }
        }
    }

    private static void OnNavigatedTo(Page toPage, INavigationParameters parameters)
    {
        MvvmHelpers.OnNavigatedTo(toPage, parameters);

        if (toPage is TabbedPage tabbedPage)
        {
            if (tabbedPage.CurrentPage is NavigationPage navigationPage)
            {
                MvvmHelpers.OnNavigatedTo(navigationPage.CurrentPage, parameters);
            }
            else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
            {
                MvvmHelpers.OnNavigatedTo(tabbedPage.CurrentPage, parameters);
            }
        }
    }

    private static void OnNavigatedFrom(Page fromPage, INavigationParameters parameters)
    {
        MvvmHelpers.OnNavigatedFrom(fromPage, parameters);

        if (fromPage is TabbedPage tabbedPage)
        {
            if (tabbedPage.CurrentPage is NavigationPage navigationPage)
            {
                MvvmHelpers.OnNavigatedFrom(navigationPage.CurrentPage, parameters);
            }
            else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
            {
                MvvmHelpers.OnNavigatedFrom(tabbedPage.CurrentPage, parameters);
            }
        }
    }

    protected virtual Page CreatePage(string segmentName)
    {
        try
        {
            var scope = _container.CreateScope();
            var page = (Page)Registry.CreateView(scope, segmentName);

            if (page is null)
                throw new NullReferenceException($"The resolved type for {segmentName} was null. You may be attempting to navigate to a Non-Page type");

            return page;
        }
        catch(NavigationException)
        {
            throw;
        }
        catch(KeyNotFoundException knfe)
        {
            throw new NavigationException(NavigationException.NoPageIsRegistered, segmentName, knfe);
        }
        catch(ViewModelCreationException vmce)
        {
            throw new NavigationException(NavigationException.ErrorCreatingViewModel, segmentName, _pageAccessor.Page, vmce);
        }
        //catch(ViewCreationException viewCreationException)
        //{
        //    if(!string.IsNullOrEmpty(viewCreationException.InnerException?.Message) && viewCreationException.InnerException.Message.Contains("Maui"))
        //        throw new NavigationException(NavigationException.)
        //}
        catch (Exception ex)
        {
            var inner = ex.InnerException;
            while(inner is not null)
            {
                if (inner.Message.Contains("thread with a dispatcher"))
                    throw new NavigationException(NavigationException.UnsupportedMauiCreation, segmentName, _pageAccessor.Page, ex);
                inner = inner.InnerException;
            }
            throw new NavigationException(NavigationException.ErrorCreatingPage, segmentName, ex);
        }
    }

    protected virtual Page CreatePageFromSegment(string segment)
    {
        string segmentName = UriParsingHelper.GetSegmentName(segment);
        var page = CreatePage(segmentName);
        if (page is null)
        {
            var innerException = new NullReferenceException(string.Format("{0} could not be created. Please make sure you have registered {0} for navigation.", segmentName));
            throw new NavigationException(NavigationException.NoPageIsRegistered, segmentName, _pageAccessor.Page, innerException);
        }

        return page;
    }

    async Task ConfigureTabbedPage(TabbedPage tabbedPage, string segment, INavigationParameters parameters)
    {
        var tabParameters = UriParsingHelper.GetSegmentParameters(segment, parameters);

        var tabsToCreate = tabParameters.GetValues<string>(KnownNavigationParameters.CreateTab);
        foreach (var tabToCreateEncoded in tabsToCreate ?? Array.Empty<string>())
        {
            //created tab can be a single view or a view nested in a NavigationPage with the syntax "NavigationPage|ViewToCreate"
            var tabToCreate = HttpUtility.UrlDecode(tabToCreateEncoded);
            var tabSegments = tabToCreate.Split('/', '|');
            NavigationPage navigationPage = null;
            for(int i = 0; i < tabSegments.Length; i++)
            {
                var tabSegment = tabSegments[i];
                var child = CreatePageFromSegment(tabSegment);
                var childParameters = UriParsingHelper.GetSegmentParameters(tabSegment, parameters);
                if (i == 0 && child is NavigationPage navPage)
                {
                    navigationPage = navPage;
                    await MvvmHelpers.OnInitializedAsync(child, childParameters);
                }
                else if(i == 0)
                {
                    tabbedPage.Children.Add(child);
                    break;
                }
                else if(i > 0 && navigationPage is not null)
                {
                    await navigationPage.Navigation.PushAsync(child);
                }
            }

            if(navigationPage is null)
            {
                continue;
            }

            tabbedPage.Children.Add(navigationPage);
            if (navigationPage.RootPage.IsSet(XamlTab.TitleProperty))
            {
                navigationPage.Title = XamlTab.GetTitle(navigationPage.RootPage);
                navigationPage.IconImageSource = XamlTab.GetIconImageSource(navigationPage.RootPage);
            }
            else if(!navigationPage.IsSet(Page.TitleProperty))
            {
                var source = navigationPage.IsSet(XamlTab.TitleBindingSourceProperty) ?
                XamlTab.GetTitleBindingSource(navigationPage) :
                navigationPage.RootPage.IsSet(XamlTab.TitleBindingSourceProperty) ?
                XamlTab.GetTitleBindingSource(navigationPage.RootPage) :
                Xaml.TabBindingSource.RootPage;

                //set the title because Xamarin doesn't do this for us.
                if (!navigationPage.IsSet(Page.TitleProperty))
                    navigationPage.SetBinding(Page.TitleProperty, new Binding($"{source}.Title", BindingMode.OneWay, source: navigationPage));
                if (!navigationPage.IsSet(Page.IconImageSourceProperty))
                    navigationPage.SetBinding(Page.IconImageSourceProperty, new Binding($"{source}.IconImageSource", BindingMode.OneWay, source: navigationPage));
            }
        }

        TabbedPageSelectTab(tabbedPage, tabParameters);
    }

    private void SelectPageTab(Page page, INavigationParameters parameters)
    {
        if (page is TabbedPage tabbedPage)
        {
            TabbedPageSelectTab(tabbedPage, parameters);
        }
    }

    private void TabbedPageSelectTab(TabbedPage tabbedPage, INavigationParameters parameters)
    {
        if (!parameters.TryGetValue<string>(KnownNavigationParameters.SelectedTab, out var selectedTab)
            || string.IsNullOrEmpty(selectedTab))
            return;

        var segments = selectedTab.Split('|').Where(x => !string.IsNullOrEmpty(x));
        if (segments.Count() == 1)
            TabbedPageSelectRootTab(tabbedPage, selectedTab);
        else if (segments.Count() > 1)
            TabbedPageSelectNavigationChildTab(tabbedPage, segments.First(), segments.Last());
    }

    private void TabbedPageSelectRootTab(TabbedPage tabbedPage, string selectedTab)
    {
        var registry = Registry;
        var selectRegistration = registry.Registrations.FirstOrDefault(x => x.Name == selectedTab);
        if (selectRegistration is null)
            throw new KeyNotFoundException($"No Registration found to select tab '{selectedTab}'.");

        var child = tabbedPage.Children
            .FirstOrDefault(x => IsPage(x, selectRegistration));
        if (child is not null)
        {
            tabbedPage.CurrentPage = child;
        }
    }

    private static bool IsPage(Page page, ViewRegistration registration) =>
        (string)page.GetValue(ViewModelLocator.NavigationNameProperty) == registration.Name || page.GetType() == registration.View;

    private void TabbedPageSelectNavigationChildTab(TabbedPage tabbedPage, string rootTab, string selectedTab)
    {
        var registry = Registry;
        var rootRegistration = registry.Registrations.FirstOrDefault(x => x.Name == rootTab);
        var selectRegistration = registry.Registrations.FirstOrDefault(x => x.Name == selectedTab);
        if (rootRegistration is null)
            throw new KeyNotFoundException($"No Registration found to select tab '{rootTab}'.");
        else if (selectRegistration is null)
            throw new KeyNotFoundException($"No Registration found to select tab '{selectedTab}'.");
        else if (!rootRegistration.View.IsAssignableTo(typeof(NavigationPage)))
            throw new InvalidOperationException($"Could not select Tab with a root type '{rootRegistration.View.FullName}'. This must inherit from NavigationPage.");

        var child = tabbedPage.Children
            .FirstOrDefault(x => x is NavigationPage navPage && IsPage(x, rootRegistration) && (IsPage(navPage.RootPage, selectRegistration) || IsPage(navPage.CurrentPage, selectRegistration)));

        if (child is not null)
            tabbedPage.CurrentPage = child;
    }

    protected virtual async Task UseReverseNavigation(Page currentPage, string nextSegment, Queue<string> segments, INavigationParameters parameters, bool? useModalNavigation, bool? animated)
    {
        var navigationStack = new Stack<string>();

        if (!string.IsNullOrWhiteSpace(nextSegment))
            navigationStack.Push(nextSegment);

        var illegalSegments = new Queue<string>();

        bool illegalPageFound = false;
        foreach (var item in segments)
        {
            //if we run into an illegal page, we need to create new navigation segments to properly handle the deep link
            if (illegalPageFound)
            {
                illegalSegments.Enqueue(item);
                continue;
            }

            //if any page decide to go modal, we need to consider it and all pages after it an illegal page
            var pageParameters = UriParsingHelper.GetSegmentParameters(item, parameters);
            if (pageParameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
            {
                if (pageParameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation))
                {
                    illegalSegments.Enqueue(item);
                    illegalPageFound = true;
                }
                else
                {
                    navigationStack.Push(item);
                }
            }
            else
            {
                var pageType = Registry.GetViewType(UriParsingHelper.GetSegmentName(item));
                if (MvvmHelpers.IsSameOrSubclassOf<FlyoutPage>(pageType))
                {
                    illegalSegments.Enqueue(item);
                    illegalPageFound = true;
                }
                else
                {
                    navigationStack.Push(item);
                }
            }
        }

        var pageOffset = currentPage.Navigation.NavigationStack.Count;
        // NOTE: Disabled due to Issue 2232
        //if (currentPage.Navigation.NavigationStack.Count > 2)
        //    pageOffset = currentPage.Navigation.NavigationStack.Count - 1;

        var onNavigatedFromTarget = currentPage;
        if (currentPage is NavigationPage navPage && navPage.CurrentPage != null)
            onNavigatedFromTarget = navPage.CurrentPage;

        bool insertBefore = false;
        while (navigationStack.Count > 0)
        {
            var segment = navigationStack.Pop();
            var nextPage = CreatePageFromSegment(segment);
            if (nextPage is TabbedPage tabbedPage)
                await ConfigureTabbedPage(tabbedPage, nextSegment, parameters);
            await DoNavigateAction(onNavigatedFromTarget, segment, nextPage, parameters, async () =>
            {
                await DoPush(currentPage, nextPage, useModalNavigation, animated, insertBefore, pageOffset);
            });
            insertBefore = true;
        }

        //if an illegal page is found, we force a Modal navigation
        if (illegalSegments.Count > 0)
            await ProcessNavigation(currentPage.Navigation.NavigationStack.Last(), illegalSegments, parameters, true, animated);
    }

    protected virtual async Task DoPush(Page currentPage, Page page, bool? useModalNavigation, bool? animated, bool insertBeforeLast = false, int navigationOffset = 0)
    {
        ArgumentNullException.ThrowIfNull(page);

        try
        {
            // Prevent Page from using Parent's ViewModel
            page.BindingContext ??= new object();

            if (currentPage is null)
            {
                if (_windowManager.Windows.OfType<PrismWindow>().Any(x => x.Name == PrismWindow.DefaultWindowName))
                    _window = _windowManager.Windows.OfType<PrismWindow>().First(x => x.Name == PrismWindow.DefaultWindowName);

                if (Window is null)
                {
                    _window = new PrismWindow
                    {
                        Page = page
                    };

                    _windowManager.OpenWindow(_window);
                }
                else
                {
                    Window.Page = page;
                }
            }
            else
            {
                bool useModalForPush = UseModalNavigation(currentPage, useModalNavigation);

                if (useModalForPush)
                {
                    await currentPage.Navigation.PushModalAsync(page, animated ?? true);
                }
                else
                {
                    if (insertBeforeLast)
                    {
                        await InsertPageBefore(currentPage, page, navigationOffset);
                    }
                    else
                    {
                        await currentPage.Navigation.PushAsync(page, animated ?? true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new NavigationException(NavigationException.UnsupportedMauiNavigation, _pageAccessor.Page, ex);
        }
    }

    protected virtual Task InsertPageBefore(Page currentPage, Page page, int pageOffset)
    {
        var firstPage = currentPage.Navigation.NavigationStack.Skip(pageOffset).FirstOrDefault();
        currentPage.Navigation.InsertPageBefore(page, firstPage);
        return Task.FromResult(true);
    }

    protected virtual Task<Page> DoPop(INavigation navigation, bool useModalNavigation, bool animated)
    {
        if (useModalNavigation)
            return navigation.PopModalAsync(animated);
        else
            return navigation.PopAsync(animated);
    }

    protected virtual Page GetCurrentPage()
    {
        return _pageAccessor.Page is not null ? _pageAccessor.Page : GetPageFromWindow();
    }

    internal static bool UseModalNavigation(Page currentPage, bool? useModalNavigationDefault)
    {
        bool useModalNavigation;
        if (useModalNavigationDefault.HasValue)
            useModalNavigation = useModalNavigationDefault.Value;
        else if (currentPage is NavigationPage)
            useModalNavigation = false;
        else
            useModalNavigation = !MvvmHelpers.HasNavigationPageParent(currentPage);

        return useModalNavigation;
    }

    internal bool UseModalGoBack(Page currentPage, INavigationParameters parameters)
    {
        if (parameters.ContainsKey(KnownNavigationParameters.UseModalNavigation))
            return parameters.GetValue<bool>(KnownNavigationParameters.UseModalNavigation);
        else if (currentPage is NavigationPage navPage)
            return GoBackModal(navPage);
        else if (MvvmHelpers.HasNavigationPageParent(currentPage, out var navParent))
            return GoBackModal(navParent);
        else
            return true;
    }

    private bool GoBackModal(NavigationPage navPage)
    {
        var rootPage = GetPageFromWindow();
        if (navPage.CurrentPage != navPage.RootPage)
            return false;
        else if (navPage.CurrentPage == navPage.RootPage && navPage.Parent is Application && rootPage != navPage)
            return true;
        else if (navPage.Parent is TabbedPage tabbed && tabbed != rootPage)
            return true;

        return false;
    }

    internal static bool UseReverseNavigation(Page currentPage, Type nextPageType)
    {
        return MvvmHelpers.HasNavigationPageParent(currentPage) && MvvmHelpers.IsSameOrSubclassOf<ContentPage>(nextPageType);
    }

    private INavigationResult Notify(NavigationRequestType type, INavigationParameters parameters, Exception exception = null)
    {
        var result = new NavigationResult(exception);
        _eventAggregator.GetEvent<NavigationRequestEvent>().Publish(new NavigationRequestContext
        {
            Parameters = parameters,
            Result = result,
            Type = type,
        });

        return result;
    }

    private INavigationResult Notify(Uri uri, INavigationParameters parameters, Exception exception = null)
    {
        var result = new NavigationResult(exception);

        var temp = Regex.Replace(uri.ToString(), RemovePageInstruction, RemovePageRelativePath);

        _eventAggregator.GetEvent<NavigationRequestEvent>().Publish(new NavigationRequestContext
        {
            Parameters = parameters,
            Result = result,
            Type = NavigationRequestType.Navigate,
            Uri = new Uri(temp, UriKind.RelativeOrAbsolute),
        });

        return result;
    }

    protected static bool IsRoot(Page mainPage, Page currentPage)
    {
        if (mainPage == currentPage)
            return true;

        return mainPage switch
        {
            FlyoutPage fp => IsRoot(fp.Detail, currentPage),
            TabbedPage tp => IsRoot(tp.CurrentPage, currentPage),
            NavigationPage np => IsRoot(np.RootPage, currentPage),
            _ => false
        };
    }

    private Page GetPageFromWindow()
    {
        try
        {
            return Window?.Page;
        }
#if DEBUG
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return null;
        }
#else
        catch
        {
            return null;
        }
#endif
    }
}
