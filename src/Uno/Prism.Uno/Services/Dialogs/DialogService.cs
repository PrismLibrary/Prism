using Prism.Common;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Prism.Services.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly IContainerProvider _containerProvider;

        public DialogService(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            ShowDialogInternal(name, parameters, callback);
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            ShowDialogInternal(name, parameters, callback, windowName);
        }

        void ShowDialogInternal(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName = null)
        {
            IDialogWindow contentDialog = CreateDialogWindow(windowName);
            ConfigureDialogWindowEvents(contentDialog, callback);
            ConfigureDialogWindowContent(name, contentDialog, parameters);

            _ = contentDialog.ShowAsync();
        }

        IDialogWindow CreateDialogWindow(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _containerProvider.Resolve<IDialogWindow>();
            else
                return _containerProvider.Resolve<IDialogWindow>(name);
        }

        void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = _containerProvider.Resolve<object>(dialogName);
            var dialogContent = content as FrameworkElement;
            if (dialogContent == null)
            {
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");
            }

            var viewModel = dialogContent.DataContext as IDialogAware;
            if (viewModel == null)
            {
                throw new NullReferenceException($"A dialog's ViewModel must implement the IDialogAware interface ({dialogContent.DataContext})");
            }

            ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
        }

        void ConfigureDialogWindowEvents(IDialogWindow contentDialog, Action<IDialogResult> callback)
        {
            IDialogResult result = null;

            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = (o) =>
            {
                result = o;
                contentDialog.Hide();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                contentDialog.Loaded -= loadedHandler;

                if (contentDialog.DataContext is IDialogAware dialogAware)
                {
                    dialogAware.RequestClose += requestCloseHandler;
                }
            };

            contentDialog.Loaded += loadedHandler;

            TypedEventHandler<ContentDialog, ContentDialogClosingEventArgs> closingHandler = null;

            closingHandler = (o, e) =>
            {
                if (contentDialog.DataContext is IDialogAware dialogAware 
                    && !dialogAware.CanCloseDialog())
                {
                    e.Cancel = true;
                }
            };

            contentDialog.Closing += closingHandler;

            TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> closedHandler = null;
            closedHandler = (o, e) =>
            {
                contentDialog.Closed -= closedHandler;
                contentDialog.Closing -= closingHandler;

                if (contentDialog.DataContext is IDialogAware dialogAware)
                {
                    dialogAware.RequestClose -= requestCloseHandler;

                    dialogAware.OnDialogClosed();
                }

                if (result == null)
                    result = new DialogResult();

                callback?.Invoke(result);

                contentDialog.DataContext = null;
                contentDialog.Content = null;
            };
            contentDialog.Closed += closedHandler;
        }

        void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            var windowStyle = Dialog.GetWindowStyle(dialogContent);
            if (windowStyle != null)
                window.Style = windowStyle;

            window.Content = dialogContent;
            window.DataContext = viewModel;
        }
    }
}
