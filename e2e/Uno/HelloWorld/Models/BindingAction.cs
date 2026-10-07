#nullable enable
using System.Windows.Input;
using Prism.Mvvm;

namespace HelloWorld.Models;

// A separately observable nested binding object, with the inherited Prism marker.
// No application Bindable or DynamicDependency workaround is applied here.
public sealed class BindingAction : BindableBase
{
    private string _text;
    private string _icon;

    public BindingAction(string text, string icon, ICommand command)
    {
        _text = text;
        _icon = icon;
        Command = command;
    }

    // Only the XAML bindings call these getters.
    public string Text => _text;
    public string Icon => _icon;
    public ICommand Command { get; }

    public void Update(string text, string icon)
    {
        SetProperty(ref _text, text, nameof(Text));
        SetProperty(ref _icon, icon, nameof(Icon));
    }
}
