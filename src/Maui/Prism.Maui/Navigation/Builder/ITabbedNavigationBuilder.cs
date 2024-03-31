using System.ComponentModel;

namespace Prism.Navigation.Builder;

/// <summary>
/// Represents a builder for configuring tabbed navigation in Prism.
/// </summary>
public interface ITabbedNavigationBuilder : INavigationBuilder
{
    /// <summary>
    /// Creates a new tab with the specified segment name or URI.
    /// </summary>
    /// <param name="segmentNameOrUri">The segment name or URI of the tab.</param>
    /// <returns>The current instance of the <see cref="ITabbedNavigationBuilder"/>.</returns>
    ITabbedNavigationBuilder CreateTab(string segmentNameOrUri);

    /// <summary>
    /// Creates a new tab with the specified view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model for the tab.</typeparam>
    /// <returns>The current instance of the <see cref="ITabbedNavigationBuilder"/>.</returns>
    ITabbedNavigationBuilder CreateTab<TViewModel>()
        where TViewModel : class, INotifyPropertyChanged;

    /// <summary>
    /// Selects the tab with the specified segment name or URI.
    /// </summary>
    /// <param name="segmentNameOrUri">The segment name or URI of the tab to select.</param>
    /// <returns>The current instance of the <see cref="ITabbedNavigationBuilder"/>.</returns>
    ITabbedNavigationBuilder SelectTab(string segmentNameOrUri);

    /// <summary>
    /// Selects the tab with the specified view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model for the tab to select.</typeparam>
    /// <returns>The current instance of the <see cref="ITabbedNavigationBuilder"/>.</returns>
    ITabbedNavigationBuilder SelectTab<TViewModel>()
        where TViewModel : class, INotifyPropertyChanged;
}
