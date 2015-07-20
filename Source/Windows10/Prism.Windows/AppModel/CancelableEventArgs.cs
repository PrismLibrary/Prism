using System;

namespace Prism.Windows.AppModel
{
    /// <summary>
    /// 
    /// </summary>
    public class CancelableEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CancelableEventArgs() { }
    }
}
