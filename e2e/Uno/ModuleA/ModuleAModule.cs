using ModuleA.Dialogs;
using ModuleA.Views;
using AlertDialog = ModuleA.Dialogs.AlertDialog;

namespace ModuleA;

public class ModuleAModule : IModule
{
    private readonly IRegionManager _regionManager;

    public ModuleAModule(IRegionManager regionManager)
    {
        _regionManager = regionManager;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ViewA>();
        containerRegistry.RegisterForNavigation<ViewB>();
        containerRegistry.RegisterDialog<AlertDialog, AlertDialogViewModel>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        //_regionManager.RegisterViewWithRegion<ViewA>("ContentRegion");
        //_regionManager.RequestNavigate("ContentRegion", "ViewA");
    }
}
