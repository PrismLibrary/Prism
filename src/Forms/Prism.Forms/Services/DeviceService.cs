using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Prism.AppModel;
using Xamarin.Forms;
using FlowDirection = Prism.AppModel.FlowDirection;

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
        /// Gets a list of custom flags that were set on the device before Xamarin.Forms was initialized.
        /// </summary>
        public IReadOnlyList<string> Flags => Device.Flags;

        /// <summary>
        /// Gets the flow direction on the device.
        /// </summary>
        public FlowDirection FlowDirection => (FlowDirection)Device.FlowDirection;

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
        public RuntimePlatform RuntimePlatform =>
            Device.RuntimePlatform switch
            {
                Android => RuntimePlatform.Android,
                GTK => RuntimePlatform.GTK,
                iOS => RuntimePlatform.iOS,
                macOS => RuntimePlatform.macOS,
                Tizen => RuntimePlatform.Tizen,
                UWP => RuntimePlatform.UWP,
                WPF => RuntimePlatform.WPF,
                _ => RuntimePlatform.Unknown,
            };

        /// <summary>
        /// Invokes an action on the device main UI thread.
        /// </summary>
        /// <param name="action">The Action to invoke</param>
        public void BeginInvokeOnMainThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

        /// <summary>
        /// Returns the current SynchronizationContext from the main thread.
        /// </summary>
        /// <returns>The current SynchronizationContext from the main thread.</returns>
        public Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync()
        {
            return Device.GetMainThreadSynchronizationContextAsync();
        }

        /// <summary>
        /// Invokes an Action on the device main (UI) thread.
        /// </summary>
        /// <param name="action">The Action to invoke</param>
        /// <returns>A task representing the work to be performed</returns>
        public Task InvokeOnMainThreadAsync(Action action)
        {
            return Device.InvokeOnMainThreadAsync(action);
        }

        /// <summary>
        /// Invokes a Func on the device main (UI) thread.
        /// </summary>
        /// <param name="func">The Func to invoke.</param>
        /// <typeparam name="T">The return type of the Func.</typeparam>
        /// <returns>A task of type T representing the work to be performed</returns>
        public Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
        {
            return Device.InvokeOnMainThreadAsync(func);
        }

        /// <summary>
        /// Invokes a Func on the device main (UI) thread.
        /// </summary>
        /// <param name="funcTask">The return type of the Func.</param>
        /// <typeparam name="T">The return type of the Func.</typeparam>
        /// <returns>A task of type T representing the work to be performed</returns>
        public Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
        {
            return Device.InvokeOnMainThreadAsync(funcTask);
        }

        /// <summary>
        /// Invokes a Func on the device main (UI) thread.
        /// </summary>
        /// <param name="funcTask">The Func to invoke.</param>
        /// <returns>A task representing the work to be performed</returns>
        public Task InvokeOnMainThreadAsync(Func<Task> funcTask)
        {
            return Device.InvokeOnMainThreadAsync(funcTask);
        }

        /// <summary>
        /// Sets a list of custom flags on the device.
        /// </summary>
        /// <param name="flags">The list of custom flag values.</param>
        public void SetFlags(IReadOnlyList<string> flags)
        {
            Device.SetFlags(flags);
        }

        /// <summary>
        /// Sets the flow direction on the device.
        /// </summary>
        /// <param name="flowDirection">The new flow direction value to set.</param>
        public void SetFlowDirection(FlowDirection flowDirection)
        {
            Device.SetFlowDirection((Xamarin.Forms.FlowDirection)flowDirection);
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
