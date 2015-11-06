

using Prism.Common;
using Prism.Interactivity.DefaultPopupWindows;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Prism.Interactivity
{
    /// <summary>
    /// Shows a popup window in response to an <see cref="InteractionRequest"/> being raised.
    /// </summary>
    public class PopupWindowAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// The content of the child window to display as part of the popup.
        /// </summary>
        public static readonly DependencyProperty WindowContentProperty =
            DependencyProperty.Register(
                "WindowContent",
                typeof(FrameworkElement),
                typeof(PopupWindowAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Determines if the content should be shown in a modal window or not.
        /// </summary>
        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register(
                "IsModal",
                typeof(bool),
                typeof(PopupWindowAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Determines if the content should be initially shown centered over the view that raised the interaction request or not.
        /// </summary>
        public static readonly DependencyProperty CenterOverAssociatedObjectProperty =
            DependencyProperty.Register(
                "CenterOverAssociatedObject",
                typeof(bool),
                typeof(PopupWindowAction),
                new PropertyMetadata(null));

        /// <summary>
        /// If set, applies this WindowStartupLocation to the child window.
        /// </summary>
        public static readonly DependencyProperty WindowStartupLocationProperty =
            DependencyProperty.Register(
                "WindowStartupLocation",
                typeof(WindowStartupLocation?),
                typeof(PopupWindowAction),
                new PropertyMetadata(null));

        /// <summary>
        /// If set, applies this Style to the child window.
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.Register(
                "WindowStyle",
                typeof(Style),
                typeof(PopupWindowAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the content of the window.
        /// </summary>
        public FrameworkElement WindowContent
        {
            get { return (FrameworkElement)GetValue(WindowContentProperty); }
            set { SetValue(WindowContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the window will be modal or not.
        /// </summary>
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the window will be initially shown centered over the view that raised the interaction request or not.
        /// </summary>
        public bool CenterOverAssociatedObject
        {
            get { return (bool)GetValue(CenterOverAssociatedObjectProperty); }
            set { SetValue(CenterOverAssociatedObjectProperty, value); }
        }

        /// <summary>
        /// Gets or sets the startup location of the Window.
        /// </summary>
        public WindowStartupLocation? WindowStartupLocation
        {
            get { return (WindowStartupLocation?)GetValue(WindowStartupLocationProperty); }
            set { SetValue(WindowStartupLocationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Style of the Window.
        /// </summary>
        public Style WindowStyle
        {
            get { return (Style)GetValue(WindowStyleProperty); }
            set { SetValue(WindowStyleProperty, value); }
        }

        /// <summary>
        /// Displays the child window and collects results for <see cref="IInteractionRequest"/>.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null)
            {
                return;
            }

            // If the WindowContent shouldn't be part of another visual tree.
            if (this.WindowContent != null && this.WindowContent.Parent != null)
            {
                return;
            }

            Window wrapperWindow = this.GetWindow(args.Context);

            // We invoke the callback when the interaction's window is closed.
            var callback = args.Callback;
            EventHandler handler = null;
            handler =
                (o, e) =>
                {
                    wrapperWindow.Closed -= handler;
                    wrapperWindow.Content = null;
                    if(callback != null) callback();
                };
            wrapperWindow.Closed += handler;

            if (this.CenterOverAssociatedObject && this.AssociatedObject != null)
            {
                // If we should center the popup over the parent window we subscribe to the SizeChanged event
                // so we can change its position after the dimensions are set.
                SizeChangedEventHandler sizeHandler = null;
                sizeHandler =
                    (o, e) =>
                    {
                        wrapperWindow.SizeChanged -= sizeHandler;

                        FrameworkElement view = this.AssociatedObject;
                        Point position = view.PointToScreen(new Point(0, 0));

                        PresentationSource source = PresentationSource.FromVisual(view);
                        position = source.CompositionTarget.TransformFromDevice.Transform(position);

                        double x = position.X + ((view.ActualWidth - wrapperWindow.ActualWidth) / 2);
                        double y = position.Y + ((view.ActualHeight - wrapperWindow.ActualHeight) / 2);

                        x = Math.Max(0, Math.Min(x, SystemParameters.WorkArea.Right - wrapperWindow.ActualWidth));
                        y = Math.Max(0, Math.Min(y, SystemParameters.WorkArea.Bottom - wrapperWindow.ActualHeight));

                        wrapperWindow.Left = x;
                        wrapperWindow.Top = y;
                    };
                wrapperWindow.SizeChanged += sizeHandler;
            }

            if (this.IsModal)
            {
                wrapperWindow.ShowDialog();
            }
            else
            {
                wrapperWindow.Show();
            }
        }

        /// <summary>
        /// Returns the window to display as part of the trigger action.
        /// </summary>
        /// <param name="notification">The notification to be set as a DataContext in the window.</param>
        /// <returns></returns>
        protected virtual Window GetWindow(INotification notification)
        {
            Window wrapperWindow;

            if (this.WindowContent != null)
            {
                wrapperWindow = CreateWindow();

                if (wrapperWindow == null)
                    throw new NullReferenceException("CreateWindow cannot return null");

                // If the WindowContent does not have its own DataContext, it will inherit this one.
                wrapperWindow.DataContext = notification;
                wrapperWindow.Title = notification.Title;

                this.PrepareContentForWindow(notification, wrapperWindow);
            }
            else
            {
                wrapperWindow = this.CreateDefaultWindow(notification);
            }

            wrapperWindow.Owner = Window.GetWindow(this);

            // If the user provided a Style for a Window we set it as the window's style.
            if (WindowStyle != null)
                wrapperWindow.Style = WindowStyle;

            // If the user has provided a startup location for a Window we set it as the window's startup location.
            if (WindowStartupLocation.HasValue)
                wrapperWindow.WindowStartupLocation = WindowStartupLocation.Value;

            return wrapperWindow;
        }

        /// <summary>
        /// Checks if the WindowContent or its DataContext implements <see cref="IInteractionRequestAware"/>.
        /// If so, it sets the corresponding value.
        /// Also, if WindowContent does not have a RegionManager attached, it creates a new scoped RegionManager for it.
        /// </summary>
        /// <param name="notification">The notification to be set as a DataContext in the HostWindow.</param>
        /// <param name="wrapperWindow">The HostWindow</param>
        protected virtual void PrepareContentForWindow(INotification notification, Window wrapperWindow)
        {
            if (this.WindowContent == null)
            {
                return;
            }

            // We set the WindowContent as the content of the window. 
            wrapperWindow.Content = this.WindowContent;

            Action<IInteractionRequestAware> setNotificationAndClose = (iira) =>
            {
                iira.Notification = notification;
                iira.FinishInteraction = () => wrapperWindow.Close();
            };

            MvvmHelpers.ViewAndViewModelAction(this.WindowContent, setNotificationAndClose);
        }

        /// <summary>
        /// Creates a Window that is used when providing custom Window Content
        /// </summary>
        /// <returns>The Window</returns>
        protected virtual Window CreateWindow()
        {
            return new DefaultWindow();
        }

        /// <summary>
        /// When no WindowContent is sent this method is used to create a default basic window to show
        /// the corresponding <see cref="INotification"/> or <see cref="IConfirmation"/>.
        /// </summary>
        /// <param name="notification">The INotification or IConfirmation parameter to show.</param>
        /// <returns></returns>
        protected Window CreateDefaultWindow(INotification notification)
        {
            Window window = null;

            if (notification is IConfirmation)
            {
                window = new DefaultConfirmationWindow() { Confirmation = (IConfirmation)notification };
            }
            else
            {
                window = new DefaultNotificationWindow() { Notification = notification };
            }

            return window;
        }
    }
}
