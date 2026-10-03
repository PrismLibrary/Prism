using Prism.Mvvm;

#nullable enable
namespace Prism.Navigation.Regions;

internal class RegionPageViewModel : BindableBase, IInitialize, INavigationAware
{
    public string? Title
    {
        get => field;
        set => SetProperty(ref field, value);
    }

    public void Initialize(INavigationParameters parameters) => UpdateTitle(parameters);

    public void OnNavigatedTo(INavigationParameters parameters) => UpdateTitle(parameters);

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
    }

    private void UpdateTitle(INavigationParameters parameters)
    {
        if (parameters.TryGetValue<string>(KnownNavigationParameters.Title, out var title))
            Title = title;
    }
}
