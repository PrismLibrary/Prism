using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Prism.Xaml;

namespace Prism.Navigation.Xaml;

public abstract class NavigationExtensionBase : TargetAwareExtensionBase<ICommand>, ICommand
{
    public static readonly BindableProperty AnimatedProperty =
        BindableProperty.Create(nameof(Animated), typeof(bool), typeof(NavigationExtensionBase), true);

    public static readonly BindableProperty UseModalNavigationProperty =
        BindableProperty.Create(nameof(UseModalNavigation), typeof(bool?), typeof(NavigationExtensionBase), null);

    protected internal bool IsNavigating { get; private set; }

    public bool Animated
    {
        get => (bool)GetValue(AnimatedProperty);
        set => SetValue(AnimatedProperty, value);
    }

    public bool? UseModalNavigation
    {
        get => (bool?)GetValue(UseModalNavigationProperty);
        set => SetValue(UseModalNavigationProperty, value);
    }

    public bool CanExecute(object parameter) => Page is not null && !IsNavigating;

    public event EventHandler CanExecuteChanged;

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

    protected override ICommand ProvideValue(IServiceProvider serviceProvider) =>
        this;

    protected virtual void Log(Exception ex, INavigationParameters parameters)
    {
        // TODO: Determine a good way to log
        Logger.LogError(ex, "Error Navigating: \n[exception]");
    }

    protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

    protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    protected void AddKnownNavigationParameters(INavigationParameters parameters)
    {
        parameters.Add(KnownNavigationParameters.Animated, Animated);
        parameters.Add(KnownNavigationParameters.UseModalNavigation, UseModalNavigation);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(Page) || propertyName == nameof(IsNavigating))
            RaiseCanExecuteChanged();
    }
}