#nullable enable
using System.ComponentModel;

namespace PrismMauiDemo.Models;

// Deliberately not BindableBase: bindings must retain inherited members of other INPC models too.
public abstract class NotifyingContext<TTitle> : INotifyPropertyChanged where TTitle : notnull
{
    private TTitle _title;
    private readonly TTitle _actionTitle;
    private bool _isNavigationVisible = true;
    private BindingAction? _actionButton;

    protected NotifyingContext(TTitle title, TTitle actionTitle)
    {
        _title = title;
        _actionTitle = actionTitle;
        _actionButton = CreateAction("Initial action", "+");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // The UI bindings are the only callers of these getters. Mutation methods below
    // keep the validation code from accidentally rooting the binding accessors.
    public TTitle Title => _title;
    public bool HasTitle => !string.IsNullOrEmpty(_title.ToString());
    public BindingAction? ActionButton => _actionButton;
    public bool HasActionButton => _actionButton is not null;
    public bool IsNavigationVisible => _isNavigationVisible;

    public void UpdateTitle(TTitle title)
    {
        if (EqualityComparer<TTitle>.Default.Equals(_title, title)) return;
        _title = title;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasTitle)));
    }

    public void UpdateAction(string text, string icon) => _actionButton?.Update(text, icon);

    public void ReplaceAction(string text, string icon)
    {
        var oldAction = _actionButton;
        _actionButton = CreateAction(text, icon);
        NotifyActionChanged();
        // Mutate the detached model too: the controls must keep the replacement.
        oldAction?.Update("Detached action", "!");
    }

    public void ClearAction()
    {
        _actionButton = null;
        NotifyActionChanged();
    }

    private BindingAction CreateAction(string text, string icon) =>
        new(text, icon, new Prism.Commands.DelegateCommand(() => UpdateTitle(_actionTitle)));

    private void NotifyActionChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActionButton)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasActionButton)));
    }

    public void ToggleNavigationVisibility()
    {
        _isNavigationVisible = !_isNavigationVisible;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNavigationVisible)));
    }
}

public sealed class BindingContext : NotifyingContext<string>
{
    public BindingContext(string title = "Initial context") : base(title, "Action command invoked") { }
}
