using Prism.Common;
using Prism.Ioc;

namespace Prism.Mvvm;

public interface IViewRegistry
{
    IEnumerable<ViewRegistration> Registrations { get; }

    object CreateView(IContainerProvider container, string name);

    Type GetViewType(string name);

    string GetViewModelNavigationKey(Type viewModelType);

    IEnumerable<ViewRegistration> ViewsOfType(Type baseType);

    bool IsRegistered(string name);
}
