using System;
using Prism.Windows.AppModel;
using Windows.Phone.UI.Input;

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
        void InitializeEventHandlers();
    }
}
