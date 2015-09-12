﻿using System;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Navigation
{
    /// <summary>
    /// A wrapper class for <see cref="NavigationEventArgs"/> providing data for
    /// navigation methods and event handlers that cannot cancel the navigation request.
    /// </summary>
    public sealed class NavigatedToEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a value that indicates the direction of movement during navigation.
        /// </summary>
        public NavigationMode NavigationMode { get; set; }

        /// <summary>
        /// Gets any Parameter object passed to the target page for the navigation.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets the data type of the source page.
        /// </summary>
        public Type SourcePageType { get; }

        /// <summary>
        /// 
        /// </summary>
        public NavigatedToEventArgs() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public NavigatedToEventArgs(NavigationEventArgs args)
        {
            NavigationMode = args.NavigationMode;
            Parameter = args.Parameter;
            SourcePageType = args.SourcePageType;
        }
    }
}
