namespace Prism.Services.Dialogs
{
    /// <summary>
    /// <see cref="IDialogWindow"/> extensions.
    /// </summary>
    internal static class IDialogWindowExtensions
    {
        /// <summary>
        /// Get the <see cref="IDialogAware"/> ViewModel from a <see cref="IDialogWindow"/>.
        /// </summary>
        /// <param name="dialogWindow"><see cref="IDialogWindow"/> to get ViewModel from.</param>
        /// <returns>ViewModel as a <see cref="IDialogAware"/>.</returns>
        internal static IDialogAware GetDialogViewModel(this IDialogWindow dialogWindow)
        {
            return (IDialogAware)dialogWindow.DataContext;
        }
    }
}
