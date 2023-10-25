using System.Linq;
using System.Windows.Input;
using Prism.Commands.Parameters;
using Prism.Ioc;
using Prism.Navigation.Regions;

namespace Prism.Commands;

/// <summary>
/// Commands for handling navigation using Xaml commands
/// </summary>
public static class NavigationCommands
{
    private static IRegionManager regionManager = ContainerLocator.Container.Resolve<IRegionManager>();

    /// <summary>
    /// Command navigates to the most recent entry in the back navigation history using xaml command.
    /// </summary>
    public static ICommand GoBackCommand { get; } =
        new DelegateCommand<string>(OnGoBackCommandExecuted);

    /// <summary>
    /// Command navigates to the most recent entry in the forward navigation history.
    /// </summary>
    public static ICommand GoForwardCommand { get; } =
        new DelegateCommand<string>(OnGoForwardCommandExecuted);

    /// <summary>
    /// Command navigation to a specific view
    /// </summary>
    public static ICommand NavigateToCommand { get; } =
        new DelegateCommand<NavigateToViewCommandParameter>(OnNavigateToCommandExecuted);

    private static void OnGoBackCommandExecuted(string obj)
    {
        if (obj == null || regionManager.Regions.All(s => s.Name != obj))
            return;

        var journal = regionManager.Regions[obj].NavigationService.Journal;

        journal.GoBack();
    }

    private static void OnGoForwardCommandExecuted(string obj)
    {
        if (obj == null || regionManager.Regions.All(s => s.Name != obj))
            return;

        var journal = regionManager.Regions[obj].NavigationService.Journal;

        journal.GoForward();
    }

    private static void OnNavigateToCommandExecuted(NavigateToViewCommandParameter obj)
    {
        if (obj == null)
            return;

        regionManager.RequestNavigate(obj.RegionName, obj.TargetView, obj.Parameters);
    }
}
