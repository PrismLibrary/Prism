using System;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.ServiceLocation;
using Prism.Common;

namespace Prism.ShowDialog
{
    public class ShowDialogAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// The content of the child window to display as part of the popup.
        /// </summary>
        public static readonly DependencyProperty WindowContentProperty =
            DependencyProperty.Register(
                "WindowContent",
                typeof(FrameworkElement),
                typeof(ShowDialogAction),
                new PropertyMetadata(null));

        /// <summary>
        /// The type of content of the child window to display as part of the popup.
        /// </summary>
        public static readonly DependencyProperty WindowContentTypeProperty =
            DependencyProperty.Register(
                "WindowContentType",
                typeof(Type),
                typeof(ShowDialogAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Determines if the content should be shown in a modal window or not.
        /// </summary>
        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register(
                "IsModal",
                typeof(bool),
                typeof(ShowDialogAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Determines if the content should be initially shown centered over the view that raised the interaction request or not.
        /// </summary>
        public static readonly DependencyProperty CenterOverAssociatedObjectProperty =
            DependencyProperty.Register(
                "CenterOverAssociatedObject",
                typeof(bool),
                typeof(ShowDialogAction),
                new PropertyMetadata(null));

        /// <summary>
        /// If set, applies this WindowStartupLocation to the child window.
        /// </summary>
        public static readonly DependencyProperty WindowStartupLocationProperty =
            DependencyProperty.Register(
                "WindowStartupLocation",
                typeof(WindowStartupLocation?),
                typeof(ShowDialogAction),
                new PropertyMetadata(null));

        /// <summary>
        /// If set, applies this Style to the child window.
        /// </summary>
        public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.Register(
                "WindowStyle",
                typeof(Style),
                typeof(ShowDialogAction),
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
        /// Gets or sets the type of content of the window.
        /// </summary>
        public Type WindowContentType
        {
            get { return (Type)GetValue(WindowContentTypeProperty); }
            set { SetValue(WindowContentTypeProperty, value); }
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
            var args = parameter as ShowDialogRequestEventArgs;
            if (args == null)
            {
                return;
            }

            // If the WindowContent shouldn't be part of another visual tree.
            if (this.WindowContent != null && this.WindowContent.Parent != null)
            {
                return;
            }

            Window wrapperWindow = new Window();

            // wrapperWindow.SizeToContent = SizeToContent.WidthAndHeight;
            // wrapperWindow.ResizeMode = ResizeMode.NoResize;

            wrapperWindow.Style = new Style
            {
                TargetType = typeof(Window),
                Setters = {
                    new Setter(Window.SizeToContentProperty, SizeToContent.WidthAndHeight),
                    new Setter(Window.ResizeModeProperty, ResizeMode.NoResize)
                }
            };

            wrapperWindow.DataContext = args.ViewModel;
            wrapperWindow.Title = args.Title;

            if (this.AssociatedObject != null)
                wrapperWindow.Owner = Window.GetWindow(AssociatedObject);

            // If the user provided a Style for a Window we set it as the window's style.
            if (this.WindowStyle != null)
                wrapperWindow.Style = WindowStyle;

            // If the user has provided a startup location for a Window we set it as the window's startup location.
            if (this.WindowStartupLocation.HasValue)
                wrapperWindow.WindowStartupLocation = WindowStartupLocation.Value;

            this.PrepareContentForWindow(wrapperWindow);

            // We invoke the callback when the interaction's window is closed.
            var callback = args.Callback;
            EventHandler handler = null;
            handler =
                (o, e) =>
                {
                    wrapperWindow.Closed -= handler;
                    wrapperWindow.Content = null;
                    if (callback != null) callback();
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

                        // If the parent window has been minimized, then the poition of the wrapperWindow is calculated to be off screen
                        // which makes it impossible to activate and bring into view.  So, we want to check to see if the parent window
                        // is minimized and automatically set the position of the wrapperWindow to be center screen.
                        var parentWindow = wrapperWindow.Owner;
                        if (parentWindow != null && parentWindow.WindowState == WindowState.Minimized)
                        {
                            wrapperWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                            return;
                        }

                        FrameworkElement view = this.AssociatedObject;

                        // Position is the top left position of the view from which the request was initiated.
                        // On multiple monitors, if the X or Y coordinate is negative it represent that the monitor from which
                        // the request was initiated is either on the left or above the PrimaryScreen
                        Point position = view.PointToScreen(new Point(0, 0));
                        PresentationSource source = PresentationSource.FromVisual(view);
                        position = source.CompositionTarget.TransformFromDevice.Transform(position);

                        // Find the middle of the calling view.
                        // Take the width and height of the view divided by 2 and add to the X and Y coordinates.
                        var middleOfView = new Point(position.X + (view.ActualWidth / 2),
                                                     position.Y + (view.ActualHeight / 2));

                        // Set the coordinates for the top left part of the wrapperWindow.
                        // Take the width of the wrapperWindow, divide it by 2 and substract it from 
                        // the X coordinate of middleOfView. Do the same thing for the Y coordinate.
                        // If the wrapper window is wider or taller than the view, it will be behind the view.
                        wrapperWindow.Left = middleOfView.X - (wrapperWindow.ActualWidth / 2);
                        wrapperWindow.Top = middleOfView.Y - (wrapperWindow.ActualHeight / 2);

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
        /// Checks if the WindowContent or its DataContext implements <see cref="IInteractionRequestAware"/>.
        /// If so, it sets the corresponding values.
        /// </summary>
        /// <param name="viewModel">The notification to be set as a DataContext in the HostWindow.</param>
        /// <param name="wrapperWindow">The HostWindow</param>
        protected virtual void PrepareContentForWindow(Window wrapperWindow)
        {
            if (this.WindowContent != null)
            {
                // We set the WindowContent as the content of the window. 
                wrapperWindow.Content = this.WindowContent;
            }
            else if (this.WindowContentType != null)
            {
                wrapperWindow.Content = ServiceLocator.Current.GetInstance(this.WindowContentType);
            }
            else
            {
                return;
            }

            Action<IShowDialogRequestAware> setNotificationAndClose = (isdra) =>
            {
                isdra.FinishInteraction = () => wrapperWindow.Close();
            };

            MvvmHelpers.ViewAndViewModelAction(wrapperWindow.Content, setNotificationAndClose);
        }
    }
}
