using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs.DefaultDialogs;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Prism.Services.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly IContainerExtension _containerExtension;

        public DialogService(IContainerExtension containerExtension)
        {
            _containerExtension = containerExtension;

            RegisterDialog<NotificationDialog, NotificationDialogViewModel>();
        }

        public void ShowNotification(string title, string message, Action<IDialogResult> callback)
        {
            var parameters = new DialogParameters($"?title={title}&message={message}");

            IDialogWindow dialogWindow = CreateDialogWindow();
            ConfigureDialogWindowEvents(dialogWindow, callback);
            ConfigureDialogWindowContent<NotificationDialog>(dialogWindow, parameters);

            //TODO: figure out how to control the parent (is this even neccessary? Should we assume the parent should be the active window?
            dialogWindow.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            //TODO: figure out how to control various properties of the window, maybe a WindowSettings object?
            //TODO: handle modal/non-modal dialogs
            dialogWindow.ShowDialog();
        }

        //TODO: create extension point to provide a custom Window
        IDialogWindow CreateDialogWindow()
        {
            return new DialogWindow();
        }

        void ConfigureDialogWindowContent<T>(IDialogWindow window, IDialogParameters parameters)
        {
            var content = _containerExtension.Resolve<T>();
            var dialogContent = content as FrameworkElement;
            if (dialogContent == null)
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            var viewModel = dialogContent.DataContext as IDialog;
            if (viewModel == null)
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialog interface");

            MvvmHelpers.ViewAndViewModelAction<IDialog>(viewModel, d => d.ProcessDialogParameters(parameters));

            window.Content = dialogContent;
            window.ViewModel = viewModel;
        }

        void ConfigureDialogWindowEvents(IDialogWindow dialogWindow, Action<IDialogResult> callback)
        {
            Action requestCloseHandler = null;
            requestCloseHandler = () =>
            {
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

                    //TODO: get dialog result from ViewModel
                    callback?.Invoke(new DialogResult());

                    dialogWindow.ViewModel = null;
                    dialogWindow.Content = null;
                };
            dialogWindow.Closed += closedHandler;
        }

        public void RegisterDialog<TView, TViewModel>() where TViewModel : IDialog
        {
            _containerExtension.Register<TView>();
            ViewModelLocationProvider.Register<TView, TViewModel>();
        }
    }
}
