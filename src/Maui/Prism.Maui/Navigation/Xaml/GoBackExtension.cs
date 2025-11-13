
namespace Prism.Navigation.Xaml;

[ContentProperty(nameof(GoBackType))]
[RequireService([typeof(IProvideValueTarget)])]
public partial class GoBackExtension : NavigationExtensionBase
{
    public static readonly BindableProperty GoBackTypeProperty =
        BindableProperty.Create(nameof(GoBackType), typeof(GoBackType), typeof(GoBackExtension), GoBackType.Default);

    public GoBackType GoBackType
    {
        get => (GoBackType)GetValue(GoBackTypeProperty);
        set => SetValue(GoBackTypeProperty, value);
    }

    protected override async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
    {
        if (GoBackType != GoBackType.ToRoot)
        {
            AddKnownNavigationParameters(parameters);
        }

        var result = GoBackType == GoBackType.ToRoot ?
                await navigationService.GoBackToRootAsync(parameters) :
                await navigationService.GoBackAsync(parameters);

        if (result.Exception != null)
        {
            Log(result.Exception, parameters);
        }
    }
}
