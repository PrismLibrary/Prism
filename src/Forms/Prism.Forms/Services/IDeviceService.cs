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
    public interface IDeviceService
    {
        /// <summary>
        /// Gets a list of custom flags that were set on the device before Xamarin.Forms was initialized.
        /// </summary>
        IReadOnlyList<string> Flags { get; }

        /// <summary>
        /// Gets the flow direction on the device.
        /// </summary>
        FlowDirection FlowDirection { get; }

        /// <summary>
        /// Gets the kind of device that Xamarin.Forms is currently working on.
        /// </summary>
        TargetIdiom Idiom { get; }

        /// <summary>
        /// Gets the Platform (OS) that the application is running on.  This is the native Device.RunTimePlatform property.
        /// </summary>
        string DeviceRuntimePlatform { get; }

        /// <summary>
        /// Gets the Platform (OS) that the application is running on. The result is an enum of type RuntimePlatform.
        /// </summary>
        RuntimePlatform RuntimePlatform { get; }

        /// <summary>
        /// Invokes an action on the device main UI thread.
        /// </summary>
        /// <param name="action">The Action to invoke</param>
        void BeginInvokeOnMainThread(Action action);

        /// <summary>
        /// Returns the current SynchronizationContext from the main thread.
        /// </summary>
        /// <returns>The current SynchronizationContext from the main thread.</returns>
        Task<SynchronizationContext> GetMainThreadSynchronizationContextAsync();

        /// <summary>
        /// Invokes an Action on the device main (UI) thread.
        /// </summary>
        /// <param name="action">The Action to invoke</param>
        /// <returns>A task representing the work to be performed</returns>
        Task InvokeOnMainThreadAsync(Action action);

        /// <summary>
        /// Invokes a Func on the device main (UI) thread.
        /// </summary>
        /// <param name="func">The Func to invoke.</param>
        /// <typeparam name="T">The return type of the Func.</typeparam>
        /// <returns>A task of type T representing the work to be performed</returns>
        Task<T> InvokeOnMainThreadAsync<T>(Func<T> func);

        /// <summary>
        /// Invokes a Func on the device main (UI) thread.
        /// </summary>
        /// <param name="funcTask">The return type of the Func.</param>
        /// <typeparam name="T">The return type of the Func.</typeparam>
        /// <returns>A task of type T representing the work to be performed</returns>
        Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask);

        /// <summary>
        /// Invokes a Func on the device main (UI) thread.
        /// </summary>
        /// <param name="funcTask">The Func to invoke.</param>
        /// <returns>A task representing the work to be performed</returns>
        Task InvokeOnMainThreadAsync(Func<Task> funcTask);

        /// <summary>
        /// Sets a list of custom flags on the device.
        /// </summary>
        /// <param name="flags">The list of custom flag values.</param>
        void SetFlags(IReadOnlyList<string> flags);

        /// <summary>
        /// Sets the flow direction on the device.
        /// </summary>
        /// <param name="flowDirection">The new flow direction value to set.</param>
        void SetFlowDirection(FlowDirection flowDirection);

        /// <summary>
        /// Starts a recurring timer using the Device clock capabilities.
        /// </summary>
        /// <param name="interval">The interval between invocations of the callback </param>
        /// <param name="callBack">Action to run when the timer elapses</param>
        void StartTimer(TimeSpan interval, Func<bool> callBack);
    }
}
