using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Common;
using Prism.Ioc;
using Windows.Foundation;

#nullable enable
namespace Prism.Dialogs
{
    /// <summary>Displays registered dialog views in a content dialog host.</summary>
    public partial class DialogService : IDialogService
    {
        private readonly IDialogViewRegistry _dialogViewRegistry;
        private readonly IContainerProvider _containerProvider;

        /// <summary>Initializes a dialog service with the application's container and dialog registrations.</summary>
        /// <param name="containerProvider">The container used to resolve the window and dialog hosts.</param>
        /// <param name="dialogViewRegistry">The registry used to create dialog views.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dialogViewRegistry"/> is null.</exception>
        public DialogService(IContainerProvider containerProvider, IDialogViewRegistry dialogViewRegistry)
        {
            _containerProvider = containerProvider;
            _dialogViewRegistry = dialogViewRegistry ?? throw new ArgumentNullException(nameof(dialogViewRegistry));
        }

        /// <inheritdoc/>
        public async void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            try
            {
                parameters ??= new DialogParameters();
                var windowName = parameters.TryGetValue<string>(KnownDialogParameters.WindowName, out var wName) ? wName : null;

                var dialogWindow = CreateDialogWindow(_containerProvider, windowName);
                if (dialogWindow is ContentDialog contentDialog)
                {
                    var windowContent = _containerProvider.Resolve<Window>().Content;
                    if (windowContent is null)
                        throw new InvalidOperationException("A dialog requires a window with content.");

                    contentDialog.XamlRoot = windowContent.XamlRoot;
                }
                ConfigureDialogWindowEvents(dialogWindow, callback);
                ConfigureDialogWindowContent(name, dialogWindow, parameters);

                var placement = GetDialogPlacement(parameters);

                await dialogWindow.ShowAsync(placement);
            }
            catch (Exception ex)
            {
                var str = ex.ToString();
                await callback.Invoke(ex);
            }
        }

        void ConfigureDialogWindowContent(string dialogName, IDialogWindow window, IDialogParameters parameters)
        {
            var content = _dialogViewRegistry.CreateView(_containerProvider, dialogName);
            if (content is not FrameworkElement dialogContent)
            {
                throw new NullReferenceException("A dialog's content must be a FrameworkElement");
            }

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
    }
}
