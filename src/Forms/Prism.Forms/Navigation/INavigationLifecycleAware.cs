using Xamarin.Forms;

namespace Prism.Navigation;

/// <summary>
/// 
/// </summary>
public interface INavigationLifecycleAware
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task OnInitializedAsync(Page page, INavigationParameters parameters);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="parameters"></param>
    void OnNavigatedTo(Page page, INavigationParameters parameters);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="parameters"></param>
    void OnNavigatedFrom(Page page, INavigationParameters parameters);
}
