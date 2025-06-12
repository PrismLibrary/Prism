using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Prism.Common;
using Prism.Ioc;

namespace Prism.Dialogs
{
    /// <summary>Implements <see cref="IDialogService"/> to show modal and non-modal dialogs.</summary>
    /// <remarks>The dialog's ViewModel must implement IDialogAware.</remarks>
    public class DialogService : IDialogService
    {
        private readonly IContainerExtension _containerExtension;

        /// <summary>Initializes a new instance of the <see cref="DialogService"/> class.</summary>
        /// <param name="containerExtension">The <see cref="IContainerExtension" /></param>
        public DialogService(IContainerExtension containerExtension)
        {
            _containerExtension = containerExtension;
        }

        /// <summary>Show dialog.</summary>
        /// <param name="name">Name of the dialog window to show.</param>
        /// <param name="parameters"><see cref="IDialogParameters"/>.</param>
        /// <param name="callback">The action to perform when the dialog is closed.</param>
        public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            parameters ??= new DialogParameters();
            var isModal = parameters.TryGetValue<bool>(KnownDialogParameters.ShowNonModal, out var show) ? !show : true;
            var windowName = parameters.TryGetValue<string>(KnownDialogParameters.WindowName, out var wName) ? wName : null;
            var owner = parameters.TryGetValue<Window>(KnownDialogParameters.ParentWindow, out var hWnd) ? hWnd : null;

            IDialogWindow dialogWindow = CreateDialogWindow(windowName);
            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent(name, dialogWindow, parameters);

            ShowDialogWindow(dialogWindow, isModal, owner);
        }

        /// <summary>Shows the dialog window.</summary>
        /// <param name="dialogWindow">The dialog window to show.</param>
        /// <param name="isModal">If true; dialog is shown as a modal</param>
        /// <param name="owner">Optional host window of the dialog. Use-case, Dialog calling a dialog.</param>
        protected virtual void ShowDialogWindow(IDialogWindow dialogWindow, bool isModal, Window owner = null)
        {
            if (isModal &&
                Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime deskLifetime)
            {
                // Ref:
                //  - https://docs.avaloniaui.net/docs/reference/controls/window#show-a-window-as-a-dialog
                //  - https://github.com/AvaloniaUI/Avalonia/discussions/7924
                // (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)

                if (owner != null)
                    dialogWindow.ShowDialog(owner);
                else
                    dialogWindow.ShowDialog(deskLifetime.MainWindow);
            }
            else
            {
                dialogWindow.Show();
            }
        }

        /// <summary>
        /// Create a new <see cref="IDialogWindow"/>.
        /// </summary>
        /// <param name="name">The name of the hosting window registered with the IContainerRegistry.</param>
        /// <returns>The created <see cref="IDialogWindow"/>.</returns>
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
        protected virtual void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = _containerExtension.Resolve<object>(dialogName);
            if (!(content is Avalonia.Controls.Control dialogContent))
                throw new NullReferenceException("A dialog's content must be an Avalonia.Controls.Control");

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
        protected virtual void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, DialogCallback callback)
        {
            Action<IDialogResult> requestCloseHandler = (result) =>
            {
                dialogWindow.Result = result;
                dialogWindow.Close();
            };

            EventHandler loadedHandler = null;

            loadedHandler = (o, e) =>
            {
                // WPF: dialogWindow.Loaded -= loadedHandler;
                dialogWindow.Opened -= loadedHandler;
                DialogUtilities.InitializeListener(dialogWindow.GetDialogViewModel(), requestCloseHandler);
            };

            dialogWindow.Opened += loadedHandler;

            EventHandler<WindowClosingEventArgs> closingHandler = null;
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
        protected virtual void ConfigureDialogWindowProperties(IDialogWindow window, Avalonia.Controls.Control dialogContent, IDialogAware viewModel)
        {
            // Avalonia returns 'null' for Dialog.GetWindowStyle(dialogContent);
            // WPF: Window > ContentControl > FrameworkElement
            // Ava: Window > WindowBase > TopLevel > Control > InputElement > Interactive > Layoutable > Visual > StyledElement.Styles (collection)

            // WPF:
            //// var windowStyle = Dialog.GetWindowStyle(dialogContent);
            //// if (windowStyle != null)
            ////     window.Style = windowStyle;

            // Make the host window and the dialog window to share the same context
            window.Content = dialogContent;
            window.DataContext = viewModel;

            // WPF:
            //// if (window.Owner == null)
            ////     window.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        }
    }
}
