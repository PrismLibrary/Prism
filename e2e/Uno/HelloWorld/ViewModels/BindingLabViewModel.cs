#nullable enable
using System.Collections.ObjectModel;
using HelloWorld.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace HelloWorld.ViewModels;

public class BindingLabViewModel : BindableBase, IRegionAware
{
    private readonly IRegionManager _navigation;
    private BindingContext? _context = new();
    private string _navigationStatus = "Opened binding lab";
    private int _revision;
    private string? _navigationRoute;

    public BindingLabViewModel(IRegionManager navigation)
    {
        _navigation = navigation;
        UpdateCommand = new DelegateCommand(() =>
        {
            if (Context is not null) Context.UpdateTitle($"Updated context {++_revision}");
            if (Items.Count > 0) Items[0].UpdateTitle($"Updated item {_revision}");
        });
        ReplaceCommand = new DelegateCommand(() => Context = new BindingContext($"Replacement {++_revision}"));
        ClearCommand = new DelegateCommand(() => Context = null);
        ToggleNavigationCommand = new DelegateCommand(() =>
        {
            if (Context is not null) Context.ToggleNavigationVisibility();
        });
        AddItemCommand = new DelegateCommand(() => Items.Add(new BindingContext($"Item {Items.Count + 1}")));
        NavigateCommand = new DelegateCommand(() => _navigation.RequestNavigate(
            "ContentRegion", "BindingLabDetail", result => NavigationStatus = result.Success
                ? "Named navigation succeeded" : $"Navigation failed: {result.Exception?.Message}"));
        BackCommand = new DelegateCommand(() => _navigation.Regions["ContentRegion"].NavigationService.Journal.GoBack());
    }

    public BindingContext? Context { get => _context; set => SetProperty(ref _context, value); }
    public ObservableCollection<BindingContext> Items { get; } = new() { new BindingContext("Initial item") };
    public string NavigationStatus { get => _navigationStatus; set => SetProperty(ref _navigationStatus, value); }
    public DelegateCommand UpdateCommand { get; }
    public DelegateCommand ReplaceCommand { get; }
    public DelegateCommand ClearCommand { get; }
    public DelegateCommand ToggleNavigationCommand { get; }
    public DelegateCommand AddItemCommand { get; }
    public DelegateCommand NavigateCommand { get; }
    public DelegateCommand BackCommand { get; }

    public bool IsNavigationTarget(NavigationContext navigationContext) =>
        string.Equals(_navigationRoute, navigationContext.Uri.ToString(), StringComparison.Ordinal);

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        _navigationRoute = navigationContext.Uri.ToString();
        NavigationStatus = $"Arrived: {_navigationRoute}";
    }
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}
