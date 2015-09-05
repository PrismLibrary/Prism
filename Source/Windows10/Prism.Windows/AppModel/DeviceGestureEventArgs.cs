using System;
using System.ComponentModel;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceGestureEventArgs : CancelEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public DeviceGestureEventArgs() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handled"></param>
        public DeviceGestureEventArgs(bool handled)
        {
            Handled = handled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handled"></param>
        public DeviceGestureEventArgs(bool handled, bool isHardwareButton)
        {
            Handled = handled;
            IsHardwareButton = isHardwareButton;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsHardwareButton { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Handled { get; set; }

    }
}
