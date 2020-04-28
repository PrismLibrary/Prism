namespace Prism.AppModel
{
    /// <summary>
    /// Represents the Platform (OS) that the application is running on.
    /// </summary>
    /// <remarks>This enum acts as a wrapper around the Device.RuntimePlatform string-based options</remarks>
    public enum RuntimePlatform
    {
        /// <summary>
        /// Google Android
        /// </summary>
        Android,

        /// <summary>
        /// GTK UI projects
        /// </summary>
        GTK,

        /// <summary>
        /// Apple IOS
        /// </summary>
        iOS,

        /// <summary>
        /// Apple macOS
        /// </summary>
        macOS,

        /// <summary>
        /// Tizen OS
        /// </summary>
        Tizen,

        /// <summary>
        /// Universal Windows Platform
        /// </summary>
        UWP,

        /// <summary>
        /// Windows Presentation Foundation
        /// </summary>
        WPF,

        /// <summary>
        /// Catchall for unlisted Platform
        /// </summary>
        Unknown
    }
}
