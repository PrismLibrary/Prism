﻿using System;
using Prism.Windows.Interfaces;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// The DeviceGestureService class is used for handling mouse, keyboard, hardware button and other gesture events.
    /// </summary>
    public class DeviceGestureService : IDeviceGestureService
    {
        public bool IsHardwareBackButtonPresent { get; private set; }
        public bool IsHardwareCameraButtonPresent { get; private set; }

        public bool UseTitleBarBackButton { get; set; }

        public event EventHandler<DeviceGestureEventArgs> GoBackRequested;
        public event EventHandler<DeviceGestureEventArgs> GoForwardRequested;
        public event EventHandler<DeviceGestureEventArgs> CameraButtonHalfPressed;
        public event EventHandler<DeviceGestureEventArgs> CameraButtonPressed;
        public event EventHandler<DeviceGestureEventArgs> CameraButtonReleased;

        /// <summary>
        /// Event helper method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler">The EventHandler</param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InvokeEvent<T>(EventHandler<T> eventHandler, object sender, T args) where T : EventArgs
        {
            EventHandler<T> handler = eventHandler;

            if (handler != null)
                handler(sender, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public DeviceGestureService()
        {
            IsHardwareBackButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "BackPressed");
            IsHardwareCameraButtonPresent = ApiInformation.IsEventPresent("Windows.Phone.UI.Input.HardwareButtons", "CameraPressed");
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeEventHandlers()
        {
            if (IsHardwareBackButtonPresent)
                HardwareButtons.BackPressed += OnHardwareButtonsBackPressed;

            if (IsHardwareCameraButtonPresent)
            {
                HardwareButtons.CameraHalfPressed += OnHardwareButtonCameraHalfPressed;
                HardwareButtons.CameraPressed += OnHardwareButtonCameraPressed;
                HardwareButtons.CameraReleased += OnHardwareButtonCameraReleased;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += OnSystemNavigationManagerBackRequested;

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += OnAcceleratorKeyActivated;

            Window.Current.CoreWindow.PointerPressed += OnPointerPressed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            InvokeEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs(e.Handled));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonsBackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            InvokeEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs(e.Handled, true));
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
                    InvokeEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs(args.Handled));
                }
                else if (virtualKey == VirtualKey.Back && winKey)
                {
                    // When Win+Backspace is pressed navigate back
                    args.Handled = true;
                    InvokeEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs(args.Handled));
                }
                else if (((int)virtualKey == 167 && noModifiers) || (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    args.Handled = true;
                    InvokeEvent<DeviceGestureEventArgs>(GoForwardRequested, this, new DeviceGestureEventArgs(args.Handled));
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
                    InvokeEvent<DeviceGestureEventArgs>(GoBackRequested, this, new DeviceGestureEventArgs(args.Handled));

                if (forwardPressed)
                    InvokeEvent<DeviceGestureEventArgs>(GoForwardRequested, this, new DeviceGestureEventArgs(args.Handled));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonCameraHalfPressed(object sender, CameraEventArgs e)
        {
            InvokeEvent<DeviceGestureEventArgs>(CameraButtonHalfPressed, this, new DeviceGestureEventArgs(true, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonCameraPressed(object sender, CameraEventArgs e)
        {
            InvokeEvent<DeviceGestureEventArgs>(CameraButtonPressed, this, new DeviceGestureEventArgs(true, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHardwareButtonCameraReleased(object sender, CameraEventArgs e)
        {
            InvokeEvent<DeviceGestureEventArgs>(CameraButtonReleased, this, new DeviceGestureEventArgs(true, true));
        }
    }
}
