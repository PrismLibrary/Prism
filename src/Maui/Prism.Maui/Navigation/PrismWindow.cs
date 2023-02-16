using Prism.AppModel;
using Prism.Common;

namespace Prism.Navigation;

internal class PrismWindow : Window
{
    public const string DefaultWindowName = "__PrismRootWindow";

    public PrismWindow(string name = DefaultWindowName)
    {
        Name = name;
        ModalPopping += PrismWindow_ModalPopping;
    }

    public string Name { get; }

    public bool IsActive { get; private set; }

    internal Page CurrentPage => Page is null ? null : MvvmHelpers.GetCurrentPage(Page);

    private async void PrismWindow_ModalPopping(object sender, ModalPoppingEventArgs e)
    {
        if (PageNavigationService.NavigationSource == PageNavigationSource.Device)
        {
            e.Cancel = true;
            var navService = Xaml.Navigation.GetNavigationService(e.Modal);
            await navService.GoBackAsync();
        }
    }

    protected override void OnActivated()
    {
        IsActive = true;
        MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(CurrentPage, x => x.IsActive = true);
    }

    protected override void OnDeactivated()
    {
        IsActive = false;
        MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(CurrentPage, x => x.IsActive = true);
    }

    protected override void OnResumed()
    {
        MvvmHelpers.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(CurrentPage, x => x.OnResume());
    }

    protected override void OnStopped()
    {
        MvvmHelpers.InvokeViewAndViewModelAction<IApplicationLifecycleAware>(CurrentPage, x => x.OnSleep());
    }
}
