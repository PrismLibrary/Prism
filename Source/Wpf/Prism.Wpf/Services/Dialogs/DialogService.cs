using Prism.Common;
using Prism.Ioc;
using Prism.Services.Dialogs.DefaultDialogs;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Prism.Services.Dialogs
{
    //TODO: figure out how to control the parent (is this even neccessary? Should we assume the parent should be the active window?
    //TODO: figure out how to control various properties of the window, maybe a WindowSettings object?
    //TODO: create extension point to provide a custom Window
    public class DialogService : IDialogService
    {
        private readonly IContainerExtension _containerExtension;

        public DialogService(IContainerExtension containerExtension)
        {
            _containerExtension = containerExtension;
        }

        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, false);
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback, true);
        }

        void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, bool isModal)
        {
            IDialogWindow dialogWindow = CreateDialogWindow();
            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent(name, dialogWindow, parameters);
            
            dialogWindow.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);

            if (isModal)
                dialogWindow.ShowDialog();
            else
                dialogWindow.Show();
        }

        IDialogWindow CreateDialogWindow()
        {
            return new DialogWindow();
        }

        void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = _containerExtension.Resolve<object>(dialogName);
            var dialogContent = content as FrameworkElement;
            if (dialogContent == null)
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            var viewModel = dialogContent.DataContext as IDialogAware;
            if (viewModel == null)
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialog interface");

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));

            window.Content = dialogContent;
            window.ViewModel = viewModel;
        }

        void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, Action<IDialogResult> callback)
        {
            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = (o) =>
            {
                dialogWindow.Result = o;
                dialogWindow.Close();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                dialogWindow.Loaded -= loadedHandler;
                dialogWindow.ViewModel.RequestClose += requestCloseHandler;
            };
            dialogWindow.Loaded += loadedHandler;

            CancelEventHandler closingHandler = null;
            closingHandler = (o, e) =>
            {
                if (!dialogWindow.ViewModel.CanCloseDialog())
                    e.Cancel = true;
            };
            dialogWindow.Closing += closingHandler;

            EventHandler closedHandler = null;
            closedHandler = (o, e) =>
                {
                    dialogWindow.Closed -= closedHandler;
                    dialogWindow.Closing -= closingHandler;
                    dialogWindow.ViewModel.RequestClose -= requestCloseHandler;

                    dialogWindow.ViewModel.OnDialogClosed();

                    if (dialogWindow.Result == null)
                        dialogWindow.Result = new DialogResult();

                    callback?.Invoke(dialogWindow.Result);

                    dialogWindow.ViewModel = null;
                    dialogWindow.Content = null;
                };
            dialogWindow.Closed += closedHandler;
        }
    }
}
