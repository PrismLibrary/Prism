using Xunit;
using Prism.Interactivity;
using Prism.Interactivity.DefaultPopupWindows;
using Prism.Interactivity.InteractionRequest;
using Prism.Wpf.Tests.Mocks;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Prism.Wpf.Tests.Interactivity
{
    
    public class PopupWindowActionFixture
    {
        [StaFact]
        public void WhenWindowContentIsNotSet_ShouldUseDefaultWindowForNotifications()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.IsModal = true;
            popupWindowAction.CenterOverAssociatedObject = true;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Assert.True(popupWindowAction.IsModal);
            Assert.True(popupWindowAction.CenterOverAssociatedObject);

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsType<DefaultNotificationWindow>(window);

            DefaultNotificationWindow defaultWindow = window as DefaultNotificationWindow;
            Assert.Same(defaultWindow.Notification, notification);
        }

        [StaFact]
        public void WhenWindowContentIsNotSet_ShouldUseDefaultWindowForConfirmations()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.IsModal = true;
            popupWindowAction.CenterOverAssociatedObject = true;

            INotification notification = new Confirmation();
            notification.Title = "Title";
            notification.Content = "Content";

            Assert.True(popupWindowAction.IsModal);
            Assert.True(popupWindowAction.CenterOverAssociatedObject);

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsType<DefaultConfirmationWindow>(window);

            DefaultConfirmationWindow defaultWindow = window as DefaultConfirmationWindow;
            Assert.Same(defaultWindow.Confirmation, notification);
        }

        [StaFact]
        public void WhenWindowContentIsSet_ShouldWrapContentInCommonWindow()
        {
            MockFrameworkElement element = new MockFrameworkElement();
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.WindowContent = element;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);
            Assert.IsNotType<DefaultNotificationWindow>(window);
            Assert.IsNotType<DefaultConfirmationWindow>(window);
            Assert.IsType<DefaultWindow>(window);
        }

        [StaFact]
        public void WhenWindowContentImplementsIInteractionRequestAware_ShouldSetUpProperties()
        {
            MockInteractionRequestAwareElement element = new MockInteractionRequestAwareElement();
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.WindowContent = element;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);
            Assert.NotNull(element.Notification);
            Assert.Same(element.Notification, notification);
            Assert.NotNull(element.FinishInteraction);
        }

        [StaFact]
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
            Assert.NotNull(dataContext.Notification);
            Assert.Same(dataContext.Notification, notification);
            Assert.NotNull(dataContext.FinishInteraction);
        }

        [StaFact]
        public void WhenStyleForWindowIsSet_WindowShouldHaveTheStyle()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            Style style = new Style(typeof(Window));
            popupWindowAction.WindowStyle = style;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);

            Assert.Same(window.Style, style);
        }

        [StaFact]      
        public void WhenStyleIsNotForWindowIsSet_InvalidOperationExceptionIsThrown()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
                Style style = new Style(typeof(StackPanel));
                popupWindowAction.WindowStyle = style;

                INotification notification = new Notification();
                notification.Title = "Title";
                notification.Content = "Content";

                Window window = popupWindowAction.GetWindow(notification);
            });

        }

        [StaFact]
        public void WhenStartupLocationForWindowIsSet_ChildWindowHasProperty()
        {
            TestablePopupWindowAction popupWindowAction = new TestablePopupWindowAction();
            popupWindowAction.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            INotification notification = new Notification();
            notification.Title = "Title";
            notification.Content = "Content";

            Window window = popupWindowAction.GetWindow(notification);

            Assert.Equal(WindowStartupLocation.CenterScreen, window.WindowStartupLocation);
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
