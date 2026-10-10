using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using Prism.Xaml;

namespace Prism.Dialogs.Xaml;

/// <summary>Creates a XAML command that opens a registered Prism dialog.</summary>
[ContentProperty(nameof(Name))]
[RequireService([typeof(IProvideValueTarget)])]
public class ShowDialogExtension : TargetAwareExtensionBase<ICommand>, ICommand
{
    /// <summary>Identifies the dialog registration name.</summary>
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(ShowDialogExtension), null);

    /// <summary>Identifies whether the command is waiting for a dialog to close.</summary>
    public static readonly BindableProperty IsExecutingProperty =
        BindableProperty.Create(nameof(IsExecuting), typeof(bool), typeof(ShowDialogExtension), false);

    /// <summary>Gets or sets the registration name of the dialog to open.</summary>
    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <summary>Gets or sets whether dialog execution is in progress.</summary>
    /// <remarks>The command cannot execute while this value is true.</remarks>
    public bool IsExecuting
    {
        get => (bool)GetValue(IsExecutingProperty);
        set => SetValue(IsExecutingProperty, value);
    }

    /// <inheritdoc/>
    public event EventHandler CanExecuteChanged;

    /// <summary>Determines whether another dialog can be opened by this command.</summary>
    /// <param name="parameter">The command parameter; it does not affect availability.</param>
    /// <returns>True when no dialog execution is in progress; otherwise false.</returns>
    public bool CanExecute(object parameter) =>
        !IsExecuting;

    /// <summary>Opens the named dialog using parameters resolved from the XAML target.</summary>
    /// <param name="parameter">The command parameter to convert to dialog parameters.</param>
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

    /// <summary>Logs any error reported when the dialog closes.</summary>
    /// <param name="result">The completed dialog result.</param>
    protected virtual void OnDialogClosed(IDialogResult result)
    {
        if (result.Exception != null)
        {
            Logger.LogWarning($"Dialog '{Name}' closed with an error:\n{result.Exception}");
        }
    }

    /// <inheritdoc/>
    protected override ICommand ProvideValue(IServiceProvider serviceProvider) =>
        this;
}
