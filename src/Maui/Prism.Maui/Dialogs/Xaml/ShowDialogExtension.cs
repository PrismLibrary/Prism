using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Xaml;

namespace Prism.Dialogs.Xaml;

[ContentProperty(nameof(Name))]
[RequireService([typeof(IProvideValueTarget)])]
public class ShowDialogExtension : TargetAwareExtensionBase<ICommand>, ICommand
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(ShowDialogExtension), null);

    public static readonly BindableProperty IsExecutingProperty =
        BindableProperty.Create(nameof(IsExecuting), typeof(bool), typeof(ShowDialogExtension), false);

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public bool IsExecuting
    {
        get => (bool)GetValue(IsExecutingProperty);
        set => SetValue(IsExecutingProperty, value);
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter) =>
        !IsExecuting;

    public void Execute(object parameter)
    {
        IsExecuting = true;
        CanExecuteChanged(this, EventArgs.Empty);

        try
        {
            var parameters = parameter.ToDialogParameters(TargetElement);
            var dialogService = Page.GetContainerProvider().Resolve<IDialogService>();
            dialogService.ShowDialog(Name, parameters, DialogClosedCallback);
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"An unexpected error occurred while showing the Dialog '{Name}'.\n{ex}");
        }
    }

    private void DialogClosedCallback(IDialogResult result)
    {
        OnDialogClosed(result);

        IsExecuting = false;
        CanExecuteChanged(this, EventArgs.Empty);
    }

    protected virtual void OnDialogClosed(IDialogResult result)
    {
        if (result.Exception != null)
        {
            Logger.LogWarning($"Dialog '{Name}' closed with an error:\n{result.Exception}");
        }
    }

    protected override ICommand ProvideValue(IServiceProvider serviceProvider) =>
        this;
}
