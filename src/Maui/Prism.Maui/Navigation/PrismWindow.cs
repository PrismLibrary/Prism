using System.ComponentModel;
using Prism.AppModel;
using Prism.Common;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Navigation.Xaml;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

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

    internal bool IsRootPage => Page switch
    {
        TabbedPage tabbed => tabbed.CurrentPage,
        NavigationPage nav => nav.RootPage,
        _ => Page
    } == CurrentPage;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnSystemBack()
    {
        var currentPage = CurrentPage;
        if(currentPage?.Parent is NavigationPage navPage)
        {
            // The NavigationPage has already taken care of the GoBack
            return;
        }

        var container = currentPage.GetContainerProvider();

        if (IsRoot(currentPage))
        {
            var app = container.Resolve<IApplication>() as Application;
            app.Quit();
            return;
        }
        else if (currentPage is IDialogContainer dialogContainer)
        {
            if (dialogContainer.Dismiss.CanExecute(null))
                dialogContainer.Dismiss.Execute(null);
        }
        else
        {
            var navigation = container.Resolve<INavigationService>();
            navigation.GoBackAsync();
        }
    }

    private bool IsRoot(Page page)
    {
        if (page == Page) return true;

        return page.Parent switch
        {
            FlyoutPage flyout => IsRoot(flyout),
            TabbedPage tabbed => IsRoot(tabbed),
            NavigationPage navigation => IsRoot(navigation),
            _ => false
        };
    }

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
