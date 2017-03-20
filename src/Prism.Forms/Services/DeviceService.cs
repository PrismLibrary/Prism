﻿using System;
using Xamarin.Forms;

namespace Prism.Services
{
    /// <summary>
    /// A service that exposes device-specific information and actions
    /// </summary>
    public class DeviceService : IDeviceService
    {
        /// <summary>
        /// Gets the kind of device that Xamarin.Forms is currently working on.
        /// </summary>
        public TargetIdiom Idiom
        {
            get { return Device.Idiom; }
        }

        /// <summary>
        /// Gets the Platform (OS) that Xamarin.Forms is working on.
        /// </summary>
        public TargetPlatform Platform
        {
            get { return Device.OS; }
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
        /// Executes different actions depending on which Platform (OS) that Xamarin.Forms is working.
        /// </summary>
        /// <param name="iOS">Action to execute when running on iOS</param>
        /// <param name="android">Action to execute when running on Android</param>
        /// <param name="winPhone">Action to execute when running on WinPhone</param>
        /// <param name="defaultAction">Action to execute if no Action was provided for the current Platform (OS)</param>
        public void OnPlatform(Action iOS = null, Action android = null, Action winPhone = null, Action defaultAction = null)
        {
            Device.OnPlatform(iOS, android, winPhone, defaultAction);
        }

        /// <summary>
        /// Returns different values depending on the Platform (OS) that Xamarin.Forms is working.
        /// </summary>
        /// <typeparam name="T">Type of value to be returned</typeparam>
        /// <param name="iOS">The value for iOS</param>
        /// <param name="android">The value for Android</param>
        /// <param name="winPhone">The value for WinPhone</param>
        /// <returns>The value for the current Platform (OS)</returns>
        public T OnPlatform<T>(T iOS, T android, T winPhone)
        {
            return Device.OnPlatform<T>(iOS, android, winPhone);
        }

        /// <summary>
        /// Request the device open a Uri.
        /// </summary>
        /// <param name="uri">The Uri to open</param>
        public void OpenUri(Uri uri)
        {
            Device.OpenUri(uri);
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
