using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Dialogs;

public class DialogViewRegistry : ViewRegistryBase, IDialogViewRegistry
{
    public DialogViewRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Dialog, registrations)
    {
    }

    protected override void ConfigureView(BindableObject bindable, IContainerProvider container)
    {
    }
}
