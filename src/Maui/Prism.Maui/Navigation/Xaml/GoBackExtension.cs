
namespace Prism.Navigation.Xaml;

/// <summary>Creates a XAML command that navigates back one page or to the navigation root.</summary>
[ContentProperty(nameof(GoBackType))]
[RequireService([typeof(IProvideValueTarget)])]
public partial class GoBackExtension : NavigationExtensionBase
{
    /// <summary>Identifies the back navigation operation to perform.</summary>
    public static readonly BindableProperty GoBackTypeProperty =
        BindableProperty.Create(nameof(GoBackType), typeof(GoBackType), typeof(GoBackExtension), GoBackType.Default);

    /// <summary>Gets or sets whether the command goes back one page or to the root.</summary>
    public GoBackType GoBackType
    {
        get => (GoBackType)GetValue(GoBackTypeProperty);
        set => SetValue(GoBackTypeProperty, value);
    }

    /// <inheritdoc/>
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
