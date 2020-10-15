using System.Linq;
using NuGet.Frameworks;
using Prism.Forms.Tests.Services.Mocks;
using Prism.Forms.Tests.Services.Mocks.Dialogs;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Prism.Services.Dialogs.Xaml;
using Xamarin.Forms;
using Xunit;

namespace Prism.Forms.Tests.Services
{
    public class DialogServiceTests
    {
        private const string DialogMockViewName = nameof(DialogMock);
        private MockApplication _currentApp;

        public DialogServiceTests()
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }

        [Fact]
        public void DoesNotThrowExceptionWithNullParametersAndCallback()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var ex = Record.Exception(() => dialogService.ShowDialog(DialogMockViewName));
            Assert.Null(ex);
            ex = Record.Exception(() => DialogMock.Current.ViewModel.SendRequestClose());
            Assert.Null(ex);
            _currentApp = null;
        }

        [Fact]
        public void DoesNotThrowExceptionWithNullParameters()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var ex = Record.Exception(() => dialogService.ShowDialog(DialogMockViewName, null, p => { }));
            Assert.Null(ex);
            ex = Record.Exception(() => DialogMock.Current.ViewModel.SendRequestClose());
            Assert.Null(ex);
            _currentApp = null;
        }

        [Fact]
        public void DoesNotThrowExceptionWithNullCallback()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var ex = Record.Exception(() => dialogService.ShowDialog(DialogMockViewName, new DialogParameters(), null));
            Assert.Null(ex);
            ex = Record.Exception(() => DialogMock.Current.ViewModel.SendRequestClose());
            Assert.Null(ex);
            _currentApp = null;
        }

        [Fact]
        public void MainPageContentPreservedOnClose()
        {
            SetMainPage();
            var mainPage = _currentApp.MainPage as ContentPage;
            var mainPageContent = mainPage.Content;
            var dialogService = CreateDialogService();
            dialogService.ShowDialog(DialogMockViewName);

            Assert.Equal(mainPage, _currentApp.MainPage);
            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);

            DialogMock.Current.ViewModel.SendRequestClose();

            Assert.Equal(mainPage, _currentApp.MainPage);
            Assert.Empty(_currentApp.MainPage.Navigation.ModalStack);
            Assert.Equal(mainPageContent, mainPage.Content);
        }

        [Fact]
        public void DefaultStyleAddedToCurrentApplication()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            dialogService.ShowDialog(DialogMockViewName);

            Assert.True(_currentApp.Resources.ContainsKey(DialogService.PopupOverlayStyle));
        }

        [Fact]
        public void CustomStyleUsedForMask()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var style = new Style(typeof(BoxView));
            DialogMock.ConstructorCallback = v => DialogLayout.SetMaskStyle(v, style);
            dialogService.ShowDialog(DialogMockViewName);

            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);
            var dialogPage = _currentApp.MainPage.Navigation.ModalStack.First() as DialogPage;
            var layout = dialogPage.Content as AbsoluteLayout;
            var mask = layout.Children[0];

            Assert.Equal(style, mask.Style);
        }

        [Fact]
        public void CanClosePreventsCallbackAndClose()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            bool didCallback = false;

            dialogService.ShowDialog(DialogMockViewName, r => didCallback = true);

            var vm = DialogMock.Current.ViewModel;
            vm.CanClose = false;

            vm.SendRequestClose();
            Assert.False(didCallback);
            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);

            vm.CanClose = true;
            vm.SendRequestClose();
            Assert.True(didCallback);
            Assert.Empty(_currentApp.MainPage.Navigation.ModalStack);
        }

        [Fact]
        public void UseMaskFalseProhibitsMaskInsersion()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            DialogMock.ConstructorCallback = v => DialogLayout.SetUseMask(v, false);
            dialogService.ShowDialog(DialogMockViewName);
            DialogMock.ConstructorCallback = null;

            var useMask = DialogLayout.GetUseMask(DialogMock.Current);
            Assert.NotNull(useMask);
            Assert.False(useMask.Value);

            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);
            var dialogPage = _currentApp.MainPage.Navigation.ModalStack.First() as DialogPage;
            Assert.NotNull(dialogPage);
            var layout = dialogPage.Content as AbsoluteLayout;

            Assert.Equal(1, layout.Children.Count);
        }

        [Fact]
        public void UseMaskTrueDefaultMaskInsersion()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            DialogMock.ConstructorCallback = v => DialogLayout.SetUseMask(v, true);
            dialogService.ShowDialog(DialogMockViewName);

            var useMask = DialogLayout.GetUseMask(DialogMock.Current);
            Assert.NotNull(useMask);
            Assert.True(useMask.Value);

            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);
            var dialogPage = _currentApp.MainPage.Navigation.ModalStack.First() as DialogPage;
            Assert.NotNull(dialogPage);
            var layout = dialogPage.Content as AbsoluteLayout;

            Assert.Equal(2, layout.Children.Count);
        }

        [Fact]
        public void CustomMaskIsInserted()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var customMask = new Image();
            DialogMock.ConstructorCallback = v => DialogLayout.SetMask(v, customMask);
            dialogService.ShowDialog(DialogMockViewName);

            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);
            var dialogPage = _currentApp.MainPage.Navigation.ModalStack.First() as DialogPage;
            Assert.NotNull(dialogPage);
            var layout = dialogPage.Content as AbsoluteLayout;

            Assert.IsType<Image>(layout.Children[0]);
        }

        [Fact]
        public void MaskDoesNotHaveDismissGestureByDefault()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            dialogService.ShowDialog(DialogMockViewName);

            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);
            var dialogPage = _currentApp.MainPage.Navigation.ModalStack.First() as DialogPage;
            Assert.NotNull(dialogPage);
            var layout = dialogPage.Content as AbsoluteLayout;
            var mask = layout.Children[0];

            Assert.Empty(mask.GestureRecognizers);
        }

        [Fact]
        public void MaskHasDismissGestureRecognizer()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            DialogMock.ConstructorCallback = v => DialogLayout.SetCloseOnBackgroundTapped(v, true);
            dialogService.ShowDialog(DialogMockViewName);

            Assert.Single(_currentApp.MainPage.Navigation.ModalStack);
            var dialogPage = _currentApp.MainPage.Navigation.ModalStack.First() as DialogPage;
            Assert.NotNull(dialogPage);
            var layout = dialogPage.Content as AbsoluteLayout;
            var mask = layout.Children[0];

            Assert.Single(mask.GestureRecognizers);
        }

        private IDialogService CreateDialogService()
        {
            if (_currentApp is null)
            {
                _currentApp = new MockApplication();
            }

            DialogMock.ConstructorCallback = null;

            return new DialogService(_currentApp, _currentApp.Container);
        }

        private void SetMainPage(Page page = null, bool resetApp = true)
        {
            if (resetApp)
            {
                _currentApp = null;
            }

            if (_currentApp is null)
            {
                _currentApp = new MockApplication();
            }

            if (page is null)
            {
                page = new ContentPage
                {
                    Content = new Label { Text = "Hello World" }
                };
            }

            _currentApp.MainPage = page;
        }
    }
}
