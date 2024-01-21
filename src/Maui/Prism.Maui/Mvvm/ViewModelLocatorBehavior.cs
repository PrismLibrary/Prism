namespace Prism.Mvvm;

/// <summary>
/// Defines the behavior that the <see cref="ViewModelLocator"/> should use.
/// </summary>
public enum ViewModelLocatorBehavior
{
    /// <summary>
    /// The ViewModel will be lazily loaded by the Page/Region Navigation Services
    /// or the DialogService.
    /// </summary>
    /// <remarks>
    /// This is the default and recommended value for the ViewModelLocator. This will
    /// allow the View to be fully initialized and ensure that the proper ViewModel is
    /// resolved based on the route name.
    /// </remarks>
    Automatic,

    /// <summary>
    /// This will disable Prism's automatic ViewModel Location
    /// </summary>
    Disabled,

    /// <summary>
    /// This is not recommended for most situations
    /// </summary>
    /// <remarks>
    /// This is likely to cause breaks in the Container Scoping. It is recommended that
    /// you allow Prism Page/Region Navigation Services or the Dialog Service properly
    /// resolve the ViewModel.
    /// </remarks>
    Forced
}
