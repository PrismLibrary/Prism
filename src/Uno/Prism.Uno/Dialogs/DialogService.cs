using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Common;
using Prism.Ioc;
using Windows.Foundation;

namespace Prism.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly IContainerProvider _containerProvider;

        public DialogService(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            parameters ??= new DialogParameters();
            var windowName = parameters.TryGetValue<string>(KnownDialogParameters.WindowName, out var wName) ? wName : null;
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
            if (!(content is FrameworkElement dialogContent))
            {
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");
            }

            MvvmHelpers.AutowireViewModel(content);

            if (!(dialogContent.DataContext is IDialogAware viewModel))
            {
                throw new NullReferenceException($"A dialog's ViewModel must implement the IDialogAware interface ({dialogContent.DataContext})");
            }

            ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
        }

        void ConfigureDialogWindowEvents(IDialogWindow contentDialog, DialogCallback callback)
        {
            IDialogResult result = null;

            Action<IDialogParameters> requestCloseHandler = null;
            requestCloseHandler = (p) =>
            {
                result = new DialogResult { Parameters = p };
                contentDialog.Hide();
            };

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (o, e) =>
            {
                contentDialog.Loaded -= loadedHandler;

                if (contentDialog.DataContext is IDialogAware dialogAware)
                {
                    dialogAware.RequestClose = new DialogCloseEvent(requestCloseHandler);
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
            closedHandler = async (o, e) =>
            {
                contentDialog.Closed -= closedHandler;
                contentDialog.Closing -= closingHandler;

                if (contentDialog.DataContext is IDialogAware dialogAware)
                {
                    dialogAware.OnDialogClosed();
                }

                if (result == null)
                    result = new DialogResult();

                await callback.Invoke(result);

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
