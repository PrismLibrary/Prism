using System;
using System.Collections.Generic;
using System.Text;
using Prism.Ioc;
using Prism.Services.Dialogs;
using Xunit;
using Prism.Forms.Tests.Mocks;
using Prism.Common;
using Prism.Forms.Tests.Services.Mocks.Dialogs;
using Xamarin.Forms;
using Prism.Forms.Tests.Services.Mocks;
using Prism.Services.Dialogs.Xaml;

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
            var dialogService = CreateDialogService();
            dialogService.ShowDialog(DialogMockViewName);

            Assert.IsType<ContentPage>(_currentApp.MainPage);
            var mainPage = _currentApp.MainPage as ContentPage;
            Assert.IsType<AbsoluteLayout>(mainPage.Content);
            Assert.True((bool)mainPage.Content.GetValue(DialogService.IsPopupHostProperty));

            DialogMock.Current.ViewModel.SendRequestClose();

            Assert.IsType<Label>(mainPage.Content);
            Assert.Equal("Hello World", ((Label)mainPage.Content).Text);
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

            var mainPage = _currentApp.MainPage as ContentPage;
            var layout = mainPage.Content as AbsoluteLayout;
            var mask = layout.Children[1];

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
            var mainPage = _currentApp.MainPage as ContentPage;
            Assert.IsType<AbsoluteLayout>(mainPage.Content);

            vm.CanClose = true;
            vm.SendRequestClose();
            Assert.True(didCallback);
            Assert.IsType<Label>(mainPage.Content);
        }

        [Fact]
        public void IAutoInitializeSetsProperties()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var title = "This message was set automagically";
            dialogService.ShowDialog(DialogMockViewName, new DialogParameters { { "title", title } });

            var dialog = DialogMock.Current;
            Assert.Equal(title, dialog.ViewModel.Title);
            var titleLabel = dialog.Children[1] as Label;
            Assert.Equal(title, titleLabel.Text);
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
            var mainPage = _currentApp.MainPage as ContentPage;
            var layout = mainPage.Content as AbsoluteLayout;

            Assert.Equal(2, layout.Children.Count);
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
            var mainPage = _currentApp.MainPage as ContentPage;
            var layout = mainPage.Content as AbsoluteLayout;

            Assert.Equal(3, layout.Children.Count);
        }

        [Fact]
        public void CustomMaskIsInserted()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            var customMask = new Image();
            DialogMock.ConstructorCallback = v => DialogLayout.SetMask(v, customMask);
            dialogService.ShowDialog(DialogMockViewName);

            var mainPage = _currentApp.MainPage as ContentPage;
            var layout = mainPage.Content as AbsoluteLayout;

            Assert.IsType<Image>(layout.Children[1]);
        }

        [Fact]
        public void MaskDoesNotHaveDismissGestureByDefault()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            dialogService.ShowDialog(DialogMockViewName);

            var mainPage = _currentApp.MainPage as ContentPage;
            var layout = mainPage.Content as AbsoluteLayout;
            var mask = layout.Children[1];

            Assert.Empty(mask.GestureRecognizers);
        }

        [Fact]
        public void MaskHasDismissGestureRecognizer()
        {
            SetMainPage();
            var dialogService = CreateDialogService();
            DialogMock.ConstructorCallback = v => DialogLayout.SetCloseOnBackgroundTapped(v, true);
            dialogService.ShowDialog(DialogMockViewName);

            var mainPage = _currentApp.MainPage as ContentPage;
            var layout = mainPage.Content as AbsoluteLayout;
            var mask = layout.Children[1];

            Assert.Single(mask.GestureRecognizers);
        }

        private IDialogService CreateDialogService()
        {
            if(_currentApp is null)
            {
                _currentApp = new MockApplication();
            }

            DialogMock.ConstructorCallback = null;

            return new DialogService(_currentApp, _currentApp.Container as IContainerExtension);
        }

        private void SetMainPage(Page page = null, bool resetApp = true)
        {
            if(resetApp)
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
