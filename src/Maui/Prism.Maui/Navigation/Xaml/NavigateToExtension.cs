using Microsoft.Extensions.Logging;

namespace Prism.Navigation.Xaml;

/// <summary>
/// A markup extension that enables navigation to a specified page by name using Prism's navigation service.
/// </summary>
[ContentProperty(nameof(Name))]
[RequireService([typeof(IProvideValueTarget)])]
public partial class NavigateToExtension : NavigationExtensionBase
{
    /// <summary>
    /// Identifies the <see cref="Name"/> bindable property.
    /// </summary>
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(NavigateToExtension), null);

    /// <summary>
    /// Gets or sets the name of the page to navigate to.
    /// </summary>
    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <inheritdoc/>
    protected override async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
    {
        AddKnownNavigationParameters(parameters);

        var result = await navigationService.NavigateAsync(Name, parameters);
        if (result.Exception != null)
        {
            Log(result.Exception, parameters);
        }
    }

    /// <inheritdoc/>
    protected override void Log(Exception ex, INavigationParameters parameters)
    {
        if (Logger.IsEnabled(LogLevel.Error))
        {
            Logger.LogError(ex, "Navigation to {PageName} failed with parameters: {Parameters}", Name, parameters);
        }
    }
}
