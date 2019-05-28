﻿using Prism.Common;
using Prism.Ioc;
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

            if (isModal)
                dialogWindow.ShowDialog();
            else
                dialogWindow.Show();
        }

        protected virtual IDialogWindow CreateDialogWindow()
        {
            return _containerExtension.Resolve<IDialogWindow>();
        }
        
        protected virutal object CreateContent()
        {
           return _containerExtension.Resolve<object>(dialogName);
        }

        void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = CreateContent();
            var dialogContent = content as FrameworkElement;
            if (dialogContent == null)
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

            var viewModel = dialogContent.DataContext as IDialogAware;
            if (viewModel == null)
                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

            ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
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
                dialogWindow.GetDialogViewModel().RequestClose += requestCloseHandler;
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
            closedHandler = (o, e) =>
                {
                    dialogWindow.Closed -= closedHandler;
                    dialogWindow.Closing -= closingHandler;
                    dialogWindow.GetDialogViewModel().RequestClose -= requestCloseHandler;

                    dialogWindow.GetDialogViewModel().OnDialogClosed();

                    if (dialogWindow.Result == null)
                        dialogWindow.Result = new DialogResult();

                    callback?.Invoke(dialogWindow.Result);

                    dialogWindow.DataContext = null;
                    dialogWindow.Content = null;
                };
            dialogWindow.Closed += closedHandler;
        }

        void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            var windowStyle = Dialog.GetWindowStyle(dialogContent);
            if (windowStyle != null)
                window.Style = windowStyle;

            window.Content = dialogContent;
            window.DataContext = viewModel; //we want the host window and the dialog to share the same data contex

            //TODO: is there a better way to set the owner
            window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
        }
    }
}
