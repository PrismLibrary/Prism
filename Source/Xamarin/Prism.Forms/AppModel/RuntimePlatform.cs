namespace Prism.AppModel
{
    /// <summary>
    /// Represents the Platform (OS) that the application is running on.
    /// </summary>
    /// <remarks>This enum acts as a wrapper around the Device.RuntimePlatform string-based options</remarks>
    public enum RuntimePlatform
    {
        Android,
        GTK,
        iOS,
        macOS,
        Tizen,
        UWP,
        WPF,
        Unknown
    }
}
