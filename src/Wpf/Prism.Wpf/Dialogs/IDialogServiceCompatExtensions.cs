namespace Prism.Dialogs
{
    /// <summary>
    /// Extensions for the IDialogService
    /// </summary>
    public static class IDialogServiceCompatExtensions
    {
#if !UNO_WINUI
        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        public static void Show(this IDialogService dialogService, string name)
        {
            Show(dialogService, name, null, null, null);
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public static void Show(this IDialogService dialogService, string name, Action<IDialogResult> callback)
        {
            Show(dialogService, name, null, callback, null);
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public static void Show(this IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            Show(dialogService, name, parameters, callback, null);
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <param name="windowName">The name of the hosting window registered with the IContainerRegistry.</param>
        public static void Show(this IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(dialogService, name, parameters, callback, windowName, true);
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <param name="windowName">The name of the hosting window registered with the IContainerRegistry.</param>
        public static void ShowDialog(this IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(dialogService, name, parameters, callback, windowName, false);
        }

        private static void ShowDialogInternal(IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName, bool isNonModal)
        {
            parameters ??= new DialogParameters();

            if (!string.IsNullOrEmpty(windowName))
                parameters.Add(KnownDialogParameters.WindowName, windowName);

            if (isNonModal)
                parameters.Add(KnownDialogParameters.ShowNonModal, true);

            dialogService.ShowDialog(name, parameters, new DialogCallback().OnClose(callback));
        }
#endif
    }
}
