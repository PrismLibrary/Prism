using Prism.Navigation;

namespace Prism.Commands.Parameters;

/// <summary>
/// The NavigateToViewCommand parameters
/// </summary>
internal class NavigateToViewCommandParameter
{
    /// <summary>
    /// The target view
    /// </summary>
    public NavigationParameters Parameters { get; set; }

    /// <summary>
    /// The navigation region name
    /// </summary>
    public string RegionName { get; set; }

    /// <summary>
    /// Target view
    /// </summary>
    public string TargetView { get; set; }
}
