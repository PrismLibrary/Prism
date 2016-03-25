using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Prism.Events;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using Prism.Windows.Tests.Mocks;
using Prism.Windows.Tests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Prism.Windows.Tests
{
    [TestClass]
    public class DeviceGestureServiceFixture
    {
        [TestMethod]
        public async Task Navigate_To_First_Page_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }


        [TestMethod]
        public async Task Navigate_To_Subsequent_Page_Back_Button_Visible()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Navigate_To_Page_With_Null_Aggregator_Still_Works()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var frame = new FrameFacadeAdapter(new Frame());
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                bool result = navigationService.Navigate("Mock", 1);

                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public async Task Navigate_To_Page_With_UseTitleBarBackButton_False_Does_Not_Change_Back_Button_Visibility()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame());
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                // If it's collapsed and we navigate, it stays collapsed
                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
                navigationService.Navigate("Mock", 1);
                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                // If it's visible and our back stack is emptied, it stays visible
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                navigationService.ClearHistory();
                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Navigate_Backwards_Can_Go_Back_Back_Button_Visible()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 3);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                Assert.IsTrue(navigationService.CanGoBack());

                navigationService.GoBack();

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Navigate_Backwards_Can_Not_Go_Back_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                Assert.IsTrue(navigationService.CanGoBack());

                navigationService.GoBack();

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Navigate_Forwards_Can_Go_Back_Back_Button_Visible()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                Assert.IsTrue(navigationService.CanGoBack());

                navigationService.GoBack();

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.GoForward();

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Clear_History_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                Assert.IsTrue(navigationService.CanGoBack());

                navigationService.ClearHistory();

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Set_State_Empty_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                frame.SetNavigationState("1,0");

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Restore_State_Back_Button_Visible()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                frame.SetNavigationState("1,2,1,34,Prism.Windows.Tests.Mocks.MockPage,4,1,1,0,34,Prism.Windows.Tests.Mocks.MockPage,4,1,2,0");

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Remove_First_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                navigationService.RemoveFirstPage();

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Remove_Last_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                navigationService.RemoveLastPage();

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }

        [TestMethod]
        public async Task Remove_All_Back_Button_Collapsed()
        {
            await DispatcherHelper.ExecuteOnUIThread(() =>
            {
                var eventAggregator = new EventAggregator();
                var frame = new FrameFacadeAdapter(new Frame(), eventAggregator);
                var sessionStateService = new MockSessionStateService();
                sessionStateService.GetSessionStateForFrameDelegate = (currentFrame) => new Dictionary<string, object>();
                var navigationService = new FrameNavigationService(frame, (pageToken) => typeof(MockPage), sessionStateService);
                var deviceGestureService = new DeviceGestureService(eventAggregator);
                deviceGestureService.UseTitleBarBackButton = true;
                var navigationManager = SystemNavigationManager.GetForCurrentView();

                // Reset back button visibility before running, can't do this in TestInitialize because CoreWindow sometimes isn't ready
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 1);

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);

                navigationService.Navigate("Mock", 2);

                Assert.AreEqual(AppViewBackButtonVisibility.Visible, navigationManager.AppViewBackButtonVisibility);

                navigationService.RemoveAllPages();

                Assert.AreEqual(AppViewBackButtonVisibility.Collapsed, navigationManager.AppViewBackButtonVisibility);
            });
        }
    }
}
