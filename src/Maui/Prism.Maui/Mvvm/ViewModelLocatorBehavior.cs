using Prism.Navigation;

namespace Prism.Mvvm;

/// <summary>
/// Defines how the ViewModelLocator should behave
/// </summary>
public enum ViewModelLocatorBehavior
{
    /// <summary>
    /// Resolves the ViewModel once the View is ready
    /// </summary>
    /// <remarks>
    /// This is the default behavior and is best for Views created by Prism Navigation.
    /// </remarks>
    Automatic,

    /// <summary>
    /// Resolves the ViewModel once we have established the correct container scope
    /// </summary>
    /// <remarks>
    /// This is best for Views such as those created as a Child of a <see cref="TabbedPage"/>
    /// where Prism Navigation will not be creating the View.
    /// </remarks>
    WhenAvailable,

    /// <summary>
    /// Immediately triggers the ViewModel resolution.
    /// </summary>
    /// <remarks>
    /// This is not compatible with Prism Navigation. You will be provided an unimplemented instance of the
    /// <see cref="INavigationService"/> if you have a dependency on it.
    /// </remarks>
    ForceLoaded,

    /// <summary>
    /// Disables Prism from Resolving the ViewModel
    /// </summary>
    Disabled
}
