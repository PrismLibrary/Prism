using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Interactivity;
using Prism.Interactivity.DefaultPopupWindows;
using Prism.Interactivity.InteractionRequest;
using Prism.Wpf.Tests.Mocks;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Prism.Wpf.Tests.Interactivity
{
    [TestClass]
    public class PopupWindowActionFixture
    {
        [TestMethod]
        public void WhenWindowContentIsNotSet_ShouldUseDefaultWindowForNotifications()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.IsModal = true;
            popupWindowAction.CenterOverAssociatedObject = true;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Assert.AreEqual(popupWindowAction.IsModal, true);
            Assert.AreEqual(popupWindowAction.CenterOverAssociatedObject, true);

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsInstanceOfType(window, typeof(DefaultNotificationWindow));

            DefaultNotificationWindow defaultWindow = window as DefaultNotificationWindow;
            Assert.ReferenceEquals(defaultWindow.Notification, notification);
        }

        [TestMethod]
        public void WhenWindowContentIsNotSet_ShouldUseDefaultWindowForConfirmations()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.IsModal = true;
            popupWindowAction.CenterOverAssociatedObject = true;

            INotification notification = new Confirmation();
            notification.Title = "Title";
            notification.Content = "Content";

            Assert.AreEqual(popupWindowAction.IsModal, true);
            Assert.AreEqual(popupWindowAction.CenterOverAssociatedObject, true);

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsInstanceOfType(window, typeof(DefaultConfirmationWindow));

            DefaultConfirmationWindow defaultWindow = window as DefaultConfirmationWindow;
            Assert.ReferenceEquals(defaultWindow.Confirmation, notification);
        }

        [TestMethod]
        public void WhenWindowContentIsSet_ShouldWrapContentInCommonWindow()
        {
            MockFrameworkElement element = new MockFrameworkElement();
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.WindowContent = element;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsNotInstanceOfType(window, typeof(DefaultNotificationWindow));
            Assert.IsNotInstanceOfType(window, typeof(DefaultConfirmationWindow));
            Assert.IsInstanceOfType(window, typeof(Window));
        }

        [TestMethod]
        public void WhenWindowContentImplementsIInteractionRequestAware_ShouldSetUpProperties()
        {
            MockInteractionRequestAwareElement element = new MockInteractionRequestAwareElement();
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.WindowContent = element;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsNotNull(element.Notification);
            Assert.ReferenceEquals(element.Notification, notification);
            Assert.IsNotNull(element.FinishInteraction);
        }

        [TestMethod]
        public void WhenDataContextOfWindowContentImplementsIInteractionRequestAware_ShouldSetUpProperties()
        {
            MockInteractionRequestAwareElement dataContext = new MockInteractionRequestAwareElement();
            MockFrameworkElement element = new MockFrameworkElement();
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            element.DataContext = dataContext;
            popupWindowAction.WindowContent = element;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsNotNull(dataContext.Notification);
            Assert.ReferenceEquals(dataContext.Notification, notification);
            Assert.IsNotNull(dataContext.FinishInteraction);
        }

        [TestMethod]
        public void WhenStyleForWindowIsSet_WindowShouldHaveTheStyle()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            Style style = new Style(typeof(Window));
            popupWindowAction.WindowStyle = style;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);

            Assert.AreSame(window.Style, style);
        }

        [TestMethod]      
        [ExpectedException(typeof(InvalidOperationException))]  
        public void WhenStyleIsNotForWindowIsSet_InvalidOperationExceptionIsThrown()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            Style style = new Style(typeof(StackPanel));
            popupWindowAction.WindowStyle = style;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);
        }

        [TestMethod]
        public void WhenStartupLocationForWindowIsSet_ChildWindowHasProperty()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);

            Assert.AreEqual(window.WindowStartupLocation, WindowStartupLocation.CenterScreen);
        }

    }

    public class TestablePopupWindowAction : PopupWindowAction
    {
        public new Window GetWindow(INotification notification)
        {
            return base.GetWindow(notification);
        }
    }
}
