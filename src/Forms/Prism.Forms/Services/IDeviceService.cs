using Prism.AppModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Services
{
    /// <summary>
    /// A service that exposes device-specific information and actions
    /// </summary>
    public interface IDeviceService
    {
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
        /// Invokes an awaitable action on the device main UI thread.
        /// </summary>
        /// <param name="action">The Action to invoke</param>
        /// <returns>A task representing the work to be performed</returns>
        Task InvokeOnMainThreadAsync(Action action);

        /// <summary>
        /// Invokes an awaitable func of type TResult on the device main UI thread.
        /// </summary>
        /// <param name="func">The func to invoke</param>
        /// <typeparam name="T">The return type of the task</typeparam>
        /// <returns>A task of type T representing the work to be performed</returns>
        Task<T> InvokeOnMainThreadAsync<T>(Func<T> func);

        /// <summary>
        /// Invokes an awaitable func of type Task TResult on the device main UI thread.
        /// </summary>
        /// <param name="funcTask">The func to invoke</param>
        /// <typeparam name="T">The return type of the task</typeparam>
        /// <returns>A task of type T representing the work to be performed</returns>
        Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask);

        /// <summary>
        /// Invokes an awaitable func of type Task on the device main UI thread.
        /// </summary>
        /// <param name="funcTask">The func to invoke</param>
        /// <returns>A task representing the work to be performed</returns>
        Task InvokeOnMainThreadAsync(Func<Task> funcTask);

        /// <summary>
        /// Starts a recurring timer using the Device clock capabilities.
        /// </summary>
        /// <param name="interval">The interval between invocations of the callback </param>
        /// <param name="callBack">Action to run when the timer elapses</param>
        void StartTimer(TimeSpan interval, Func<bool> callBack);
    }
}
