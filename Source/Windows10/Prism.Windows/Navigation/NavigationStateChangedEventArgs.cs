using System;

namespace Prism.Windows.Navigation
{
    public enum StateChangeType
    {
        None, Back, Forward, Clear, Remove, Set
    }

    public class NavigationStateChangedEventArgs
    {
        private readonly WeakReference<IFrameFacade> _frameFacadeWeakRef;

        /// <summary>
        /// Creates a new NavigationStateChangedEventArgs instance given the frame the event pertains to and the change that occurred.
        /// </summary>
        /// <param name="frameFacade">The frame that is raising the event.</param>
        /// <param name="stateChange">The type of state change that occurred.</param>
        public NavigationStateChangedEventArgs(IFrameFacade frameFacade, StateChangeType stateChange)
        {
            _frameFacadeWeakRef = new WeakReference<IFrameFacade>(frameFacade);
            StateChange = stateChange;
        }

        /// <summary>
        /// The Frame that raised the event.
        /// </summary>
        public IFrameFacade Sender
        {
            get
            {
                IFrameFacade frameFacade = null;
                if (_frameFacadeWeakRef.TryGetTarget(out frameFacade))
                {
                    return frameFacade;
                }

                return null;
            }
        }

        /// <summary>
        /// The type of state change that occurred.
        /// </summary>
        public StateChangeType StateChange { get; private set; }
    }
}