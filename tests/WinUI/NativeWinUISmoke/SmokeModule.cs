using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;

namespace NativeWinUISmoke;

public sealed class SmokeModule : IModule
{
    private readonly SmokeState _state;

    public SmokeModule(SmokeState state) => _state = state;

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        _state.ModuleRegisterTypesCount++;
        containerRegistry.RegisterForNavigation<ModuleView>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        _state.ModuleInitializedCount++;
        containerProvider.Resolve<IRegionViewRegistry>()
            .RegisterViewWithRegion(SmokeShell.NestedRegionName, typeof(DefaultView));
    }
}
