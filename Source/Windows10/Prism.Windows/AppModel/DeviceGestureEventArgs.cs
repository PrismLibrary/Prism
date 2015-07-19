using System;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceGestureEventArgs : EventArgs
    {
        public bool IsHardwareButton { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Handled { get; set; }

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
    }
}
