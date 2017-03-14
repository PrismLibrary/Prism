using System;
using Windows.UI.Xaml.Navigation;

namespace Prism.Windows.Navigation
{
    /// <summary>
    /// A wrapper class for <see cref="NavigatingCancelEventArgs"/> providing data for
    /// navigation methods and event handlers that can be used to cancel a navigation request from origination.
    /// </summary>
    public sealed class NavigatingFromEventArgs : EventArgs
    {
        private readonly NavigatingCancelEventArgs _eventArgs;

        /// <summary>
        /// Specifies whether a pending navigation should be canceled.
        /// </summary>
        public bool Cancel
        {
            get { return _eventArgs != null ? _eventArgs.Cancel : false; }
            set
            {
                if (_eventArgs != null)
                    _eventArgs.Cancel = value;
            }
        }

        /// <summary>
        /// Gets the value of the mode parameter from the originating Navigate call.
        /// </summary>
        public NavigationMode NavigationMode { get; set; }

        /// <summary>
        /// Gets the navigation parameter associated with this navigation.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets the value of the SourcePageType parameter from the originating Navigate call.
        /// </summary>
        public Type SourcePageType { get; }

        /// <summary>
        /// Creates a new <see cref="NavigatingFromEventArgs"/> instance.
        /// </summary>
        public NavigatingFromEventArgs() { }

        /// <summary>
        /// Creates a new <see cref="NavigatingFromEventArgs"/> instance based on <see cref="NavigatingCancelEventArgs"/>.
        /// </summary>
        /// <param name="args"></param>
        public NavigatingFromEventArgs(NavigatingCancelEventArgs args)
        {
            _eventArgs = args;

            NavigationMode = args.NavigationMode;
            Parameter = args.Parameter;
            SourcePageType = args.SourcePageType;
        }
    }
}
