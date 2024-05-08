using Prism.Common;
using Xamarin.Forms;

namespace Prism.Navigation;

/// <summary>
/// 
/// </summary>
public class NavigationLifecycleAware : INavigationLifecycleAware
{
    /// <inheritdoc />
    async Task INavigationLifecycleAware.OnInitializedAsync(Page page, INavigationParameters parameters)
    {
        await OnInitializedAsyncInternal(page, parameters);
        await OnInitializedAsync(page, parameters);
    }
    
    void INavigationLifecycleAware.OnNavigatedTo(Page page, INavigationParameters parameters)
    {
        OnNavigatedToInternal(page, parameters);
        OnNavigatedTo(page, parameters);
    }
    
    void INavigationLifecycleAware.OnNavigatedFrom(Page page, INavigationParameters parameters)
    {
        OnNavigatedFromInternal(page, parameters);
        OnNavigatedFrom(page, parameters);
    }
    
    /// <inheritdoc cref="M:INavigationLifecycleAware.OnInitializedAsync"/>
    public virtual Task OnInitializedAsync(Page page, INavigationParameters parameters) => Task.CompletedTask;
    
    /// <inheritdoc cref="M:INavigationLifecycleAware.OnNavigatedTo"/>
    public virtual void OnNavigatedTo(Page toPage, INavigationParameters parameters) => NoOp();
    
    /// <inheritdoc cref="M:INavigationLifecycleAware.OnNavigatedFrom"/>
    public virtual void OnNavigatedFrom(Page toPage, INavigationParameters parameters) => NoOp();
    
    private static async Task OnInitializedAsyncInternal(Page toPage, INavigationParameters parameters)
    {
        await PageUtilities.OnInitializedAsync(toPage, parameters);
        
        if (toPage is TabbedPage tabbedPage)
        {
            foreach (var child in tabbedPage.Children)
            {
                if (child is NavigationPage navigationPage)
                {
                    await PageUtilities.OnInitializedAsync(navigationPage.CurrentPage, parameters);
                }
                else
                {
                    await PageUtilities.OnInitializedAsync(child, parameters);
                }
            }
        }
        else if (toPage is CarouselPage carouselPage)
        {
            foreach (var child in carouselPage.Children)
            {
                await PageUtilities.OnInitializedAsync(child, parameters);
            }
        }
    }
    
    private static void OnNavigatedToInternal(Page toPage, INavigationParameters parameters)
    {
        PageUtilities.OnNavigatedTo(toPage, parameters);
        
        if (toPage is TabbedPage tabbedPage && tabbedPage.CurrentPage != null)
        {
            if (tabbedPage.CurrentPage is NavigationPage navigationPage)
            {
                PageUtilities.OnNavigatedTo(navigationPage.CurrentPage, parameters);
            }
            else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
            {
                PageUtilities.OnNavigatedTo(tabbedPage.CurrentPage, parameters);
            }
        }
        else if (toPage is CarouselPage carouselPage)
        {
            PageUtilities.OnNavigatedTo(carouselPage.CurrentPage, parameters);
        }
    }
    
    private static void OnNavigatedFromInternal(Page fromPage, INavigationParameters parameters)
    {
        PageUtilities.OnNavigatedFrom(fromPage, parameters);
        
        if (fromPage is TabbedPage tabbedPage && tabbedPage.CurrentPage != null)
        {
            if (tabbedPage.CurrentPage is NavigationPage navigationPage)
            {
                PageUtilities.OnNavigatedFrom(navigationPage.CurrentPage, parameters);
            }
            else if (tabbedPage.BindingContext != tabbedPage.CurrentPage.BindingContext)
            {
                PageUtilities.OnNavigatedFrom(tabbedPage.CurrentPage, parameters);
            }
        }
        else if (fromPage is CarouselPage carouselPage)
        {
            PageUtilities.OnNavigatedFrom(carouselPage.CurrentPage, parameters);
        }
    }
    
    private static void NoOp()
    {
    }
}
