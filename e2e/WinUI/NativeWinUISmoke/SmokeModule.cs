using Prism.Ioc;
using Prism.Modularity;

namespace NativeWinUISmoke;

public sealed class SmokeModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry) =>
        containerRegistry.RegisterForNavigation<ModuleView>();

    public void OnInitialized(IContainerProvider containerProvider) =>
        containerProvider.Resolve<SmokeState>().ModuleInitialized = true;
}
