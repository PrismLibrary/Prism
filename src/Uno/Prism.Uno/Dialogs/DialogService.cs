using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Common;
using Prism.Ioc;
using Windows.Foundation;

#nullable enable
namespace Prism.Dialogs
{
    public class DialogService : IDialogService
    {
        private readonly IContainerProvider _containerProvider;

        public DialogService(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public async void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            try
            {
                parameters ??= new DialogParameters();
                var windowName = parameters.TryGetValue<string>(KnownDialogParameters.WindowName, out var wName) ? wName : null;

                var dialogWindow = CreateDialogWindow(windowName);
                if (dialogWindow is ContentDialog contentDialog)
                {
                    contentDialog.XamlRoot = _containerProvider.Resolve<Window>().Content.XamlRoot;
                }
                ConfigureDialogWindowEvents(dialogWindow, callback);
                ConfigureDialogWindowContent(name, dialogWindow, parameters);

                var placement = parameters.ContainsKey(KnownDialogParameters.DialogPlacement) ?
                    (parameters[KnownDialogParameters.DialogPlacement] is ContentDialogPlacement placementValue ? placementValue : Enum.Parse<ContentDialogPlacement>(parameters[KnownDialogParameters.DialogPlacement].ToString() ?? string.Empty)) : ContentDialogPlacement.Popup;

                await dialogWindow.ShowAsync(placement);
            }
            catch (Exception ex)
            {
                var str = ex.ToString();
                await callback.Invoke(ex);
            }
        }

        IDialogWindow CreateDialogWindow(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _containerProvider.Resolve<IDialogWindow>();
            else
                return _containerProvider.Resolve<IDialogWindow>(name);
        }

        void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = _containerProvider.Resolve<object>(dialogName);
            if (content is not FrameworkElement dialogContent)
            {
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");
            }

            MvvmHelpers.AutowireViewModel(content);

            if (dialogContent.DataContext is not IDialogAware viewModel)
            {
                throw new NullReferenceException($"A dialog's ViewModel must implement the IDialogAware interface ({dialogContent.DataContext})");
            }

            DialogService.ConfigureDialogWindowProperties(window, dialogContent, viewModel);

            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));
        }

        void ConfigureDialogWindowEvents(IDialogWindow contentDialog, DialogCallback callback)
        {
            IDialogResult? result = null;

            void RequestCloseHandler(IDialogResult r)
            {
                result = r ?? new DialogResult();
                contentDialog.Hide();
            }

            RoutedEventHandler loadedHandler = null!;
            loadedHandler = (o, e) =>
            {
                contentDialog.Loaded -= loadedHandler;

                if (contentDialog.DataContext is IDialogAware dialogAware)
                {
                    DialogUtilities.InitializeListener(dialogAware, RequestCloseHandler);
                }
            };

            contentDialog.Loaded += loadedHandler;

            void ClosingHandler(ContentDialog o, ContentDialogClosingEventArgs e)
            {
                if (contentDialog.DataContext is IDialogAware dialogAware
                    && !dialogAware.CanCloseDialog())
                {
                    e.Cancel = true;
                }
            }

            contentDialog.Closing += ClosingHandler;

            TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> closedHandler = null!;
            closedHandler = async (o, e) =>
            {
                contentDialog.Closed -= closedHandler;
                contentDialog.Closing -= ClosingHandler;

                if (contentDialog.DataContext is IDialogAware dialogAware)
                {
                    dialogAware.OnDialogClosed();
                }

                result ??= new DialogResult();

                await callback.Invoke(result);

                contentDialog.DataContext = null;
                contentDialog.Content = null;
            };
            contentDialog.Closed += closedHandler;
        }

        private static void ConfigureDialogWindowProperties(IDialogWindow window, FrameworkElement dialogContent, IDialogAware viewModel)
        {
            var windowStyle = Dialog.GetWindowStyle(dialogContent);
            if (windowStyle != null)
                window.Style = windowStyle;

            window.Content = dialogContent;
            window.DataContext = viewModel;
        }
    }
}
