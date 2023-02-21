namespace HelloWorld.ViewModels;

public class ShellViewModel : BindableBase
{
    public string Title { get; set; } = "Hello Prism for Uno.WinUI";

    private readonly IRegionManager _regionManager;

    public DelegateCommand<string> NavigateCommand { get; }

    public ShellViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        NavigateCommand = new DelegateCommand<string>(ExecuteNavigateCommand);
    }

    private void ExecuteNavigateCommand(string viewName)
    {
        _regionManager.RequestNavigate("ContentRegion", viewName);
    }
}
