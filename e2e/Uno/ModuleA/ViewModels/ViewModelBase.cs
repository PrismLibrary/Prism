using System.Collections.ObjectModel;

namespace ModuleA.ViewModels;

internal class ViewModelBase : BindableBase, IRegionAware
{
    private readonly string _name;
    protected IRegionManager RegionManager { get; }

    protected ViewModelBase(IRegionManager regionManager)
    {
        RegionManager = regionManager;
        Title = _name = GetType().Name.Replace("ViewModel", string.Empty);
    }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private ObservableCollection<string> _messages = new();
    public IEnumerable<string> Messages => _messages;


    public bool IsNavigationTarget(NavigationContext navigationContext) =>
        navigationContext.NavigatedName() == _name;

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
        _messages.Add("OnNavigatedFrom");
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.TryGetValue("title", out string title))
        {
            Title = title;
        }

        _messages.Add("OnNavigatedTo");
    }
}
