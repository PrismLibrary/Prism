namespace Prism.Navigation.Xaml;

/// <summary>
/// Specifies the type of navigation to perform when going back in the navigation stack.
/// </summary>
public enum GoBackType
{
    /// <summary>
    /// Performs the default go back operation, navigating to the previous page in the stack.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Navigates back to the root page of the navigation stack.
    /// </summary>
    ToRoot = 1
}
