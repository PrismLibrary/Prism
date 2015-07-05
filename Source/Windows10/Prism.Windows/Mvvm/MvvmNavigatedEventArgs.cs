using System;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Mvvm
{
    /// <summary>
    /// Provides data for navigation methods and event handlers that cannot cancel the navigation request.
    /// </summary>
    public class MvvmNavigatedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value that indicates the direction of movement during navigation.
        /// </summary>
        /// 
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        public NavigationMode NavigationMode { get; set; }

        /// <summary>
        /// Gets any Parameter object passed to the target page for the navigation.
        /// </summary>
        /// 
        /// <returns>
        /// An object that potentially passes parameters to the navigation target. May be null.
        /// </returns>
        public object Parameter { get; set; }
    }
}