#nullable enable
using System.Collections.ObjectModel;
using PrismMauiDemo.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace PrismMauiDemo.ViewModels;

public class BindingLabViewModel : BindableBase, INavigationAware
{
    private readonly INavigationService _navigation;
    private BindingContext? _context = new();
    private string _navigationStatus = "Opened binding lab";
    private int _revision;

    public BindingLabViewModel(INavigationService navigation)
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
        NavigateCommand = new AsyncDelegateCommand(async () =>
        {
            var result = await _navigation.NavigateAsync("BindingLabDetail");
            NavigationStatus = result.Success ? "Named navigation succeeded" : $"Navigation failed: {result.Exception?.Message}";
        });
        BackCommand = new AsyncDelegateCommand(async () =>
        {
            var result = await _navigation.GoBackAsync();
            NavigationStatus = result.Success ? "Back succeeded" : $"Back failed: {result.Exception?.Message}";
        });
        HomeCommand = new AsyncDelegateCommand(async () =>
        {
            var result = await _navigation.NavigateAsync("/RootPage");
            NavigationStatus = result.Success ? "Home succeeded" : $"Home failed: {result.Exception?.Message}";
        });
    }

    public BindingContext? Context { get => _context; set => SetProperty(ref _context, value); }
    public ObservableCollection<BindingContext> Items { get; } = new() { new BindingContext("Initial item") };
    public string NavigationStatus { get => _navigationStatus; set => SetProperty(ref _navigationStatus, value); }
    public DelegateCommand UpdateCommand { get; }
    public DelegateCommand ReplaceCommand { get; }
    public DelegateCommand ClearCommand { get; }
    public DelegateCommand ToggleNavigationCommand { get; }
    public DelegateCommand AddItemCommand { get; }
    public AsyncDelegateCommand NavigateCommand { get; }
    public AsyncDelegateCommand BackCommand { get; }
    public AsyncDelegateCommand HomeCommand { get; }

    public void OnNavigatedTo(INavigationParameters parameters) => NavigationStatus = "Arrived through Prism navigation";
    public void OnNavigatedFrom(INavigationParameters parameters) { }
}
