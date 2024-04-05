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
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public static void Show(this IDialogService dialogService, string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            parameters = EnsureShowNonModalParameter(parameters);
            dialogService.ShowDialog(name, parameters, new DialogCallback().OnClose(callback));
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
            parameters = EnsureShowNonModalParameter(parameters);

            if(!string.IsNullOrEmpty(windowName))
                parameters.Add(KnownDialogParameters.WindowName, windowName);

            dialogService.ShowDialog(name, parameters, new DialogCallback().OnClose(callback));
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        public static void Show(this IDialogService dialogService, string name)
        {
            var parameters = EnsureShowNonModalParameter(null);
            dialogService.Show(name, parameters, null);
        }

        /// <summary>
        /// Shows a non-modal dialog.
        /// </summary>
        /// <param name="dialogService">The DialogService</param>
        /// <param name="name">The name of the dialog to show.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public static void Show(this IDialogService dialogService, string name, Action<IDialogResult> callback)
        {
            var parameters = EnsureShowNonModalParameter(null);
            dialogService.Show(name, parameters, callback);
        }

        private static IDialogParameters EnsureShowNonModalParameter(IDialogParameters parameters)
        {
            parameters ??= new DialogParameters();

            if (!parameters.ContainsKey(KnownDialogParameters.ShowNonModal))
                parameters.Add(KnownDialogParameters.ShowNonModal, true);

            return parameters;
        }
#endif
    }
}
