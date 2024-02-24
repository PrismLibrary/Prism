using Prism.Mvvm;

namespace Prism.Dialogs;

/// <summary>
/// Implementation of a view registry specifically for dialog views.
/// </summary>
public class DialogViewRegistry : ViewRegistryBase, IDialogViewRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogViewRegistry"/> class with the specified view registrations.
    /// </summary>
    /// <param name="registrations">The collection of view registrations to manage.</param>
    public DialogViewRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Dialog, registrations)
    {
    }

    /// <summary>
    /// Configures a dialog view with the specified context and container provider.
    /// </summary>
    /// <param name="bindable">The bindable object representing the dialog view to configure.</param>
    /// <param name="container">The container provider to use for resolving dependencies.</param>
    protected override void ConfigureView(BindableObject bindable, IContainerProvider container)
    {
    }
}
