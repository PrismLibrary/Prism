using Prism.AppModel;
using System;
using Xamarin.Forms;

namespace Prism.Services
{
    /// <summary>
    /// A service that exposes device-specific information and actions
    /// </summary>
    public class DeviceService : IDeviceService
    {
        public const string Android = "Android";
        public const string GTK = "GTK";
        public const string iOS = "iOS";
        public const string macOS = "macOS";
        public const string Tizen = "Tizen";
        public const string UWP = "UWP";
        public const string WPF = "WPF";


        /// <summary>
        /// Gets the kind of device that Xamarin.Forms is currently working on.
        /// </summary>
        public TargetIdiom Idiom => Device.Idiom;

        /// <summary>
        /// Gets the Platform (OS) that the application is running on.  This is the native Device.RunTimePlatform property.
        /// </summary>
        public string DeviceRuntimePlatform => Device.RuntimePlatform;

        /// <summary>
        /// Gets the Platform (OS) that the application is running on. The result is an enum of type RuntimePlatform.
        /// </summary>
        public RuntimePlatform RuntimePlatform
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Android:
                        return RuntimePlatform.Android;
                    case GTK:
                        return RuntimePlatform.GTK;
                    case iOS:
                        return RuntimePlatform.iOS;
                    case macOS:
                        return RuntimePlatform.macOS;
                    case Tizen:
                        return RuntimePlatform.Tizen;
                    case UWP:
                        return RuntimePlatform.UWP;
                    case WPF:
                        return RuntimePlatform.WPF;
                    default:
                        return RuntimePlatform.Unknown;
                }
            }
        }

        /// <summary>
        /// Invokes an action on the device main UI thread.
        /// </summary>
        /// <param name="action">The Action to invoke</param>
        public void BeginInvokeOnMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

        /// <summary>
        /// Starts a recurring timer using the Device clock capabilities.
        /// </summary>
        /// <param name="interval">The interval between invocations of the callback </param>
        /// <param name="callBack">Action to run when the timer elapses</param>
        public void StartTimer(TimeSpan interval, Func<bool> callBack)
        {
            Device.StartTimer(interval, callBack);
        }
    }
}
