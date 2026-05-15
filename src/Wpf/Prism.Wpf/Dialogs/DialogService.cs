using System.ComponentModel;
using Prism.Common;

namespace Prism.Dialogs
{
    /// <summary>
    /// Implements <see cref="IDialogService"/> to show modal and non-modal dialogs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DialogService"/> is responsible for managing the lifecycle of dialog windows in WPF applications.
    /// It handles dialog creation, initialization, showing/hiding, and event coordination between the view and view model.
    /// </para>
    /// <para>
    /// The dialog's ViewModel must implement <see cref="IDialogAware"/> to participate in the dialog lifecycle.
    /// Dialog views must also implement <see cref="IDialogWindow"/> or inherit from it.
    /// </para>
    /// </remarks>
    public class DialogService : IDialogService
    {
        private readonly IContainerExtension _containerExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="containerExtension">The <see cref="IContainerExtension" /> used to resolve dialog views and windows.</param>
        public DialogService(IContainerExtension containerExtension)
        {
            _containerExtension = containerExtension;
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="name">The name of the dialog to show. This name must be registered in the container.</param>
        /// <param name="parameters">The parameters to pass to the dialog's ViewModel.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <remarks>
        /// The dialog can be shown as either modal or non-modal based on the <see cref="KnownDialogParameters.ShowNonModal"/> parameter.
        /// By default, dialogs are shown as modal (blocking the parent window).
        /// </remarks>
        public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            parameters ??= new DialogParameters();
            var isModal = parameters.TryGetValue<bool>(KnownDialogParameters.ShowNonModal, out var show) ? !show : true;
            var windowName = parameters.TryGetValue<string>(KnownDialogParameters.WindowName, out var wName) ? wName : null;

            IDialogWindow dialogWindow = CreateDialogWindow(windowName);
            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent(name, dialogWindow, parameters);

            ShowDialogWindow(dialogWindow, isModal);
        }

        /// <summary>
        /// Shows the dialog window.
        /// </summary>
        /// <param name="dialogWindow">The dialog window to show.</param>
        /// <param name="isModal">If <see langword="true"/>, the dialog is shown as a modal window that blocks interaction with the parent; otherwise, it is shown as a modeless window.</param>
        /// <remarks>
        /// This method can be overridden to customize how the dialog window is displayed.
        /// </remarks>
        protected virtual void ShowDialogWindow(IDialogWindow dialogWindow, bool isModal)
        {
            if (isModal)
                dialogWindow.ShowDialog();
            else
                dialogWindow.Show();
        }

        /// <summary>
        /// Create a new <see cref="IDialogWindow"/>.
        /// </summary>
        /// <param name="name">The name of the hosting window registered with the IContainerRegistry. If <see langword="null"/> or empty, the default dialog window is used.</param>
        /// <returns>The created <see cref="IDialogWindow"/>.</returns>
        /// <remarks>
        /// This method can be overridden to customize dialog window creation.
        /// </remarks>
        protected virtual IDialogWindow CreateDialogWindow(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _containerExtension.Resolve<IDialogWindow>();
            else
                return _containerExtension.Resolve<IDialogWindow>(name);
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> content.
        /// </summary>
        /// <param name="dialogName">The name of the dialog to show.</param>
        /// <param name="window">The hosting window.</param>
        /// <param name="parameters">The parameters to pass to the dialog.</param>
        /// <remarks>
        /// This method resolves the dialog content from the container, auto-wires the view model, and initializes the dialog.
        /// </remarks>
        protected virtual void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = _containerExtension.Resolve<object>(dialogName);
            if (!(content is FrameworkElement dialogContent))
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            MvvmHelpers.AutowireViewModel(dialogContent);

            if (!(dialogContent.DataContext is IDialogAware viewModel))
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

            ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> and <see cref="IDialogAware"/> events.
        /// </summary>
        /// <param name="dialogWindow">The hosting window.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        /// <remarks>
        /// This method sets up event handlers for the dialog lifecycle including Loaded, Closing, and Closed events.
        /// </remarks>
        protected virtual void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, DialogCallback callback)
        {
            Action<IDialogResult> requestCloseHandler = (r) =>
            {
                dialogWindow.Result = r;
                dialogWindow.Close();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                dialogWindow.Loaded -= loadedHandler;
                DialogUtilities.InitializeListener(dialogWindow.GetDialogViewModel(), requestCloseHandler);
            };
            dialogWindow.Loaded += loadedHandler;

            CancelEventHandler closingHandler = null;
            closingHandler = (o, e) =>
            {
                if (!dialogWindow.GetDialogViewModel().CanCloseDialog())
                    e.Cancel = true;
            };
            dialogWindow.Closing += closingHandler;

            EventHandler closedHandler = null;
            closedHandler = async (o, e) =>
                {
                    dialogWindow.Closed -= closedHandler;
                    dialogWindow.Closing -= closingHandler;

                    dialogWindow.GetDialogViewModel().OnDialogClosed();

                    if (dialogWindow.Result == null)
                        dialogWindow.Result = new DialogResult();

                    await callback.Invoke(dialogWindow.Result);

                    dialogWindow.DataContext = null;
                    dialogWindow.Content = null;
                };
            dialogWindow.Closed += closedHandler;
        }

        /// <summary>
        /// Configure <see cref="IDialogWindow"/> properties.
        /// </summary>
        /// <param name="window">The hosting window.</param>
        /// <param name="dialogContent">The dialog to show.</param>
        /// <param name="viewModel">The dialog's ViewModel.</param>
        /// <remarks>
        /// This method sets up the window style, title, and content based on the dialog view and view model configuration.
        /// </remarks>
        protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            var windowStyle = Dialog.GetWindowStyle(dialogContent);
            if (windowStyle != null)
                window.Style = windowStyle;


            window.Content = dialogContent;
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data context

            if (window.Owner == null)
                window.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        }
    }
}
