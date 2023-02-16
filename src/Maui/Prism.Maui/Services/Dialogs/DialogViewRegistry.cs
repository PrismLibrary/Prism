using Prism.Ioc;
using Prism.Mvvm;
using Prism.Common;

namespace Prism.Services;

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
