
namespace MauiModule.ViewModels;

public class ViewAViewModel : ViewModelBase
{
    public ViewAViewModel(BaseServices baseServices) 
        : base(baseServices)
    {
    }

    public bool CanNavigateResult { get; set; }

    public override bool CanNavigate(INavigationParameters parameters)
    {
        return CanNavigateResult;
    }
}
