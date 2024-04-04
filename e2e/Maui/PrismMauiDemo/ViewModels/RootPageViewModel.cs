using MauiModule.ViewModels;

namespace PrismMauiDemo.ViewModels;

public class RootPageViewModel
{
    private INavigationService _navigationService { get; }

    public RootPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateCommand = new AsyncDelegateCommand<string>(OnNavigateCommandExecuted);
    }

    public AsyncDelegateCommand<string> NavigateCommand { get; }

    private async Task OnNavigateCommandExecuted(string uri)
    {
        if (uri == "TabbedPage")
        {
            await _navigationService.CreateBuilder()
                .AddTabbedSegment(s => s.CreateTab<ViewAViewModel>()
                    .CreateTab(t => t.AddNavigationPage().AddSegment<ViewBViewModel>())
                    .CreateTab("ViewC")
                    .CreateTab("ViewD"))
                .NavigateAsync();
        }
        else
        {
            _navigationService.NavigateAsync(uri)
                .OnNavigationError(ex => Console.WriteLine(ex));
        }
    }
}
