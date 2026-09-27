using Prism.Mvvm;

namespace Prism.Dialogs;

/// <summary>
/// Creates the views registered for dialogs.
/// </summary>
public class DialogViewRegistry : ViewRegistryBase, IDialogViewRegistry
{
    /// <summary>
    /// Initializes the dialog view registry.
    /// </summary>
    /// <param name="registrations">The available view registrations.</param>
    public DialogViewRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Dialog, registrations)
    {
    }
}
