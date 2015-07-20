﻿using System;
using Prism.Windows.AppModel;
using Windows.Devices.Input;

namespace Prism.Windows.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceGestureService
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsHardwareBackButtonPresent { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsHardwareCameraButtonPresent { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsKeyboardPresent { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsMousePresent { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsTouchPresent { get; }

        /// <summary>
        /// 
        /// </summary>
        bool UseTitleBarBackButton { get; set; }

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<DeviceGestureEventArgs> GoBackRequested;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<DeviceGestureEventArgs> GoForwardRequested;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<DeviceGestureEventArgs> CameraButtonHalfPressed;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<DeviceGestureEventArgs> CameraButtonPressed;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<DeviceGestureEventArgs> CameraButtonReleased;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<MouseEventArgs> MouseMoved;

        /// <summary>
        /// 
        /// </summary>
        void InitializeEventHandlers();
    }
}
