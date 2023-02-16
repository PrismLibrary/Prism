using Prism.Common;

namespace MauiRegionsModule.ViewModels;

public class RegionViewAViewModel : RegionViewModelBase, IInitialize
{
    public RegionViewAViewModel(INavigationService navigationService, IPageAccessor pageAccessor) 
        : base(navigationService, pageAccessor)
    {
    }

    public void Initialize(INavigationParameters parameters)
    {
        if (parameters.TryGetValue<string>("Message", out var message))
            Message = message;
    }
}

