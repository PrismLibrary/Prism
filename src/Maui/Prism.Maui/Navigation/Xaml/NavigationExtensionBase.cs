using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Prism.Xaml;

namespace Prism.Navigation.Xaml;

/// <summary>
/// Provides a base class for navigation extensions in XAML, implementing <see cref="ICommand"/> for navigation actions.
/// </summary>
public abstract class NavigationExtensionBase : TargetAwareExtensionBase<ICommand>, ICommand
{
    /// <summary>
    /// Identifies the <see cref="Animated"/> bindable property.
    /// </summary>
    public static readonly BindableProperty AnimatedProperty =
        BindableProperty.Create(nameof(Animated), typeof(bool), typeof(NavigationExtensionBase), true);

    /// <summary>
    /// Identifies the <see cref="UseModalNavigation"/> bindable property.
    /// </summary>
    public static readonly BindableProperty UseModalNavigationProperty =
        BindableProperty.Create(nameof(UseModalNavigation), typeof(bool?), typeof(NavigationExtensionBase), null);

    /// <summary>
    /// Gets a value indicating whether a navigation operation is currently in progress.
    /// </summary>
    protected internal bool IsNavigating { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether navigation should be animated.
    /// </summary>
    public bool Animated
    {
        get => (bool)GetValue(AnimatedProperty);
        set => SetValue(AnimatedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether modal navigation should be used.
    /// </summary>
    public bool? UseModalNavigation
    {
        get => (bool?)GetValue(UseModalNavigationProperty);
        set => SetValue(UseModalNavigationProperty, value);
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data, this object can be set to null.</param>
    /// <returns>true if the command can execute; otherwise, false.</returns>
    public bool CanExecute(object parameter) => Page is not null && !IsNavigating;

    /// <inheritdoc/>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Executes the navigation command.
    /// </summary>
    /// <param name="parameter">The navigation parameters.</param>
    public async void Execute(object parameter)
    {
        var parameters = parameter.ToNavigationParameters(TargetElement);

        IsNavigating = true;
        try
        {
            var navigationService = Navigation.GetNavigationService(Page);
            await HandleNavigation(parameters, navigationService);
        }
        catch (Exception ex)
        {
            Log(ex, parameters);
        }
        finally
        {
            IsNavigating = false;
        }
    }

    /// <summary>
    /// Provides the value of the extension, which is the command itself.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The command instance.</returns>
    protected override ICommand ProvideValue(IServiceProvider serviceProvider) =>
        this;

    /// <summary>
    /// Logs navigation errors.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="parameters">The navigation parameters.</param>
    protected virtual void Log(Exception ex, INavigationParameters parameters)
    {
        if (Logger.IsEnabled(LogLevel.Error))
        {
            Logger.LogError(ex, "Error Navigating: \n[exception]");
        }
    }

    /// <summary>
    /// Handles the navigation logic. Must be implemented by derived classes.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    /// <param name="navigationService">The navigation service.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Adds known navigation parameters such as <see cref="KnownNavigationParameters.Animated"/> and <see cref="KnownNavigationParameters.UseModalNavigation"/> to the provided <see cref="INavigationParameters"/>.
    /// </summary>
    /// <param name="parameters">The navigation parameters to which known values will be added.</param>
    protected void AddKnownNavigationParameters(INavigationParameters parameters)
    {
        parameters.Add(KnownNavigationParameters.Animated, Animated);

        if (UseModalNavigation.HasValue)
            parameters.Add(KnownNavigationParameters.UseModalNavigation, UseModalNavigation);
    }

    /// <summary>
    /// Called when a property value changes.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(Page) || propertyName == nameof(IsNavigating))
            RaiseCanExecuteChanged();
    }
}
