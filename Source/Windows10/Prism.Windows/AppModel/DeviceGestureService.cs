using Prism.Events;
using Prism.Windows.Navigation;
using System;
using System.ComponentModel;
using Windows.Devices.Input;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// The DeviceGestureService class is used for handling mouse, 
    /// keyboard, hardware button and other gesture events.
    /// </summary>
    public class DeviceGestureService : IDeviceGestureService, IDisposable
    {
        private SubscriptionToken _navigationStateChangedEventToken;
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// 
        /// </summary>
        public DeviceGestureService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SubscribeToNavigationStateChanges();
            IsHardwareBackButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "BackPressed");
            IsHardwareCameraButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "CameraPressed");

            IsKeyboardPresent = new KeyboardCapabilities().KeyboardPresent != 0;
            IsMousePresent = new MouseCapabilities().MousePresent != 0;
            IsTouchPresent = new TouchCapabilities().TouchPresent != 0;

            if (IsHardwareBackButtonPresent)
                HardwareButtons.BackPressed += OnHardwareButtonsBackPressed;

            if (IsHardwareCameraButtonPresent)
            {
                HardwareButtons.CameraHalfPressed += OnHardwareButtonCameraHalfPressed;
                HardwareButtons.CameraPressed += OnHardwareButtonCameraPressed;
                HardwareButtons.CameraReleased += OnHardwareButtonCameraReleased;
            }

            if (IsMousePresent)
                MouseDevice.GetForCurrentView().MouseMoved += OnMouseMoved;

            SystemNavigationManager.GetForCurrentView().BackRequested += OnSystemNavigationManagerBackRequested;

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;

            Window.Current.CoreWindow.PointerPressed += OnPointerPressed;

        }

        public bool IsHardwareBackButtonPresent { get; private set; }
        public bool IsHardwareCameraButtonPresent { get; private set; }

        public bool IsKeyboardPresent { get; private set; }
        public bool IsMousePresent { get; private set; }
        public bool IsTouchPresent { get; private set; }
        public bool UseTitleBarBackButton { get; set; }


        /// <summary>
        /// The handlers attached to GoBackRequested are invoked in reverse order
        /// so that handlers added by the users are invoked before handlers in the system.
        /// </summary>
        public event EventHandler<DeviceGestureEventArgs> GoBackRequested;

        /// <summary>
        /// The handlers attached to GoForwardRequested are invoked in reverse order
        /// so that handlers added by the users are invoked before handlers in the system.
        /// </summary>
        public event EventHandler<DeviceGestureEventArgs> GoForwardRequested;
        public event EventHandler<DeviceGestureEventArgs> CameraButtonHalfPressed;
        public event EventHandler<DeviceGestureEventArgs> CameraButtonPressed;
        public event EventHandler<DeviceGestureEventArgs> CameraButtonReleased;
        public event EventHandler<MouseEventArgs> MouseMoved;

        /// <summary>
        /// Dispose implementation - unsubscribes from nav state changes
        /// </summary>
        public void Dispose()
        {
            UnsubscribeFromNavigationStateChanges();
        }

        /// <summary>
        /// Invokes the handlers attached to an eventhandler.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler">The EventHandler</param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void RaiseEvent<T>(EventHandler<T> eventHandler, object sender, T args)
        {
            EventHandler<T> handler = eventHandler;

            if (handler != null)
                foreach (EventHandler<T> del in handler.GetInvocationList())
                {
                    try
                    {
                        del(sender, args);
                    }
                    catch { } // Events should be fire and forget, subscriber fail should not affect publishing process
                }
        }

        /// <summary>
        /// Invokes the handlers attached to an eventhandler in reverse order and stops if a handler has canceled the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void RaiseCancelableEvent<T>(EventHandler<T> eventHandler, object sender, T args) where T : CancelEventArgs
        {
            EventHandler<T> handler = eventHandler;

            if (handler != null)
            {
                Delegate[] invocationList = handler.GetInvocationList();

                for (int i = invocationList.Length - 1; i >= 0; i--)
                {
                    EventHandler<T> del = (EventHandler<T>)invocationList[i];

                    try
                    {
                        del(sender, args);

                        if (args.Cancel)
                            break;
                    }
                    catch { } // Events should be fire and forget, subscriber fail should not affect publishing process
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnMouseMoved(MouseDevice sender, MouseEventArgs args)
        {
            RaiseEvent<MouseEventArgs>(MouseMoved, this, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            DeviceGestureEventArgs args = new DeviceGestureEventArgs();

            RaiseCancelableEvent<DeviceGestureEventArgs>(GoBackRequested, this, args);

            e.Handled = args.Handled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            DeviceGestureEventArgs args = new DeviceGestureEventArgs(false, true);

            RaiseCancelableEvent<DeviceGestureEventArgs>(GoBackRequested, this, args);

            e.Handled = args.Handled;
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations.
        /// Used to detect keyboard navigation between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="args">Event data describing the conditions that led to the event.</param>
        protected virtual void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                args.EventType == CoreAcceleratorKeyEventType.KeyDown))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                var virtualKey = args.VirtualKey;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool winKey = ((coreWindow.GetKeyState(VirtualKey.LeftWindows) & downState) == downState || (coreWindow.GetKeyState(VirtualKey.RightWindows) & downState) == downState);
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey && !winKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey && !winKey;

                //TODO: DeviceGestureService.KeyDown event?

                if (((int)virtualKey == 166 && noModifiers) || (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    args.Handled = true;
                    RaiseCancelableEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs());
                }
                else if (virtualKey == VirtualKey.Back && winKey)
                {
                    // When Win+Backspace is pressed navigate back
                    args.Handled = true;
                    RaiseCancelableEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs());
                }
                else if (((int)virtualKey == 167 && noModifiers) || (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    args.Handled = true;
                    RaiseCancelableEvent<DeviceGestureEventArgs>(GoForwardRequested, this, new DeviceGestureEventArgs());
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction.
        /// Used to detect browser-style next and previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="args">Event data describing the conditions that led to the event.</param>
        protected virtual void OnPointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed || properties.IsMiddleButtonPressed)
                return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;

            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;

                if (backPressed)
                    RaiseCancelableEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs());

                if (forwardPressed)
                    RaiseCancelableEvent<DeviceGestureEventArgs>(GoForwardRequested, this, new DeviceGestureEventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonCameraHalfPressed(object sender, CameraEventArgs e)
        {
            RaiseEvent<DeviceGestureEventArgs>(CameraButtonHalfPressed, this, new DeviceGestureEventArgs(false, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonCameraPressed(object sender, CameraEventArgs e)
        {
            RaiseEvent<DeviceGestureEventArgs>(CameraButtonPressed, this, new DeviceGestureEventArgs(false, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonCameraReleased(object sender, CameraEventArgs e)
        {
            RaiseEvent<DeviceGestureEventArgs>(CameraButtonReleased, this, new DeviceGestureEventArgs(false, true));
        }

        private void SubscribeToNavigationStateChanges()
        {
            var navigationStateChangedEvent = _eventAggregator.GetEvent<NavigationStateChangedEvent>();
            _navigationStateChangedEventToken = navigationStateChangedEvent.Subscribe(OnNavigationStateChanged);
        }

        private void UnsubscribeFromNavigationStateChanges()
        {
            _eventAggregator.GetEvent<NavigationStateChangedEvent>().Unsubscribe(_navigationStateChangedEventToken);
        }

        private void OnNavigationStateChanged(NavigationStateChangedEventArgs e)
        {
            if (UseTitleBarBackButton && e.Sender != null)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    e.Sender.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }
        }
    }
}
