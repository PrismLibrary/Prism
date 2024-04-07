using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiModule.ViewModels;

public abstract class ViewModelBase : BindableBase, IInitialize, INavigatedAware, IPageLifecycleAware
{
    protected INavigationService _navigationService { get; }
    protected IPageDialogService _pageDialogs { get; }
    protected IDialogService _dialogs { get; }

    protected ViewModelBase(BaseServices baseServices)
    {
        _navigationService = baseServices.NavigationService;
        _pageDialogs = baseServices.PageDialogs;
        _dialogs = baseServices.Dialogs;
        Title = Regex.Replace(GetType().Name, "ViewModel", string.Empty);
        Id = Guid.NewGuid().ToString();
        NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
        ShowPageDialog = new DelegateCommand(OnShowPageDialog);
        Messages = new ObservableCollection<string>();
        Messages.CollectionChanged += (sender, args) =>
        {
            foreach (string message in args.NewItems)
                Console.WriteLine($"{Title} - {message}");
        };

        AvailableDialogs = baseServices.DialogRegistry.Registrations.Select(x => x.Name).ToList();
        SelectedDialog = AvailableDialogs.FirstOrDefault();
        ShowDialog = new DelegateCommand(OnShowDialogCommand, () => !string.IsNullOrEmpty(SelectedDialog))
            .ObservesProperty(() => SelectedDialog);
        GoBack = new DelegateCommand<string>(OnGoToBack);
    }

    public IEnumerable<string> AvailableDialogs { get; }

    public string Title { get; }

    public string Id { get; }

    private string _selectedDialog;
    public string SelectedDialog
    {
        get => _selectedDialog;
        set => SetProperty(ref _selectedDialog, value);
    }

    public ObservableCollection<string> Messages { get; }

    public DelegateCommand<string> NavigateCommand { get; }

    public DelegateCommand ShowPageDialog { get; }

    public DelegateCommand ShowDialog { get; }

    public DelegateCommand<string> GoBack { get; }

    private void OnNavigateCommandExecuted(string uri)
    {
        Messages.Add($"OnNavigateCommandExecuted: {uri}");
        _navigationService.NavigateAsync(uri)
            .OnNavigationError(ex => Console.WriteLine(ex));
    }

    private void OnShowPageDialog()
    {
        Messages.Add("OnShowPageDialog");
        _pageDialogs.DisplayAlertAsync("Message", $"Hello from {Title}. This is a Page Dialog Service Alert!", "Ok");
    }

    private void OnShowDialogCommand()
    {
        Messages.Add("OnShowDialog");
        _dialogs.ShowDialog(SelectedDialog, null, DialogCallback);
    }

    private void DialogCallback(IDialogResult result) =>
        Messages.Add("Dialog Closed");

    private void OnGoToBack(string viewName)
    {
        Messages.Add($"On Go Back {viewName}");
        _navigationService.GoBackToAsync(viewName);
    }

    public void Initialize(INavigationParameters parameters)
    {
        Messages.Add("ViewModel Initialized");
        foreach (var parameter in parameters.Where(x => x.Key.Contains("message")))
            Messages.Add(parameter.Value.ToString());
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        Messages.Add("ViewModel NavigatedFrom");
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        Messages.Add("ViewModel NavigatedTo");
    }

    public void OnAppearing()
    {
        Messages.Add("View Appearing");
    }

    public void OnDisappearing()
    {
        Messages.Add("View Disappearing");
    }
}
