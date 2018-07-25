using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Prism
{
    public abstract partial class PrismApplicationBase : IPrismApplicationEvents
    {
#pragma warning disable CS0067 // unused events
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event EventHandler<object> Resuming;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event SuspendingEventHandler Suspending;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event EnteredBackgroundEventHandler EnteredBackground;
        [EditorBrowsable(EditorBrowsableState.Never)]
        private new event LeavingBackgroundEventHandler LeavingBackground;
#pragma warning restore CS0067

        EnteredBackgroundEventHandler _enteredBackground;
        event EnteredBackgroundEventHandler IPrismApplicationEvents.EnteredBackground
        {
            add { _enteredBackground += value; }
            remove { _enteredBackground -= value; }
        }
        LeavingBackgroundEventHandler _leavingBackground;
        event LeavingBackgroundEventHandler IPrismApplicationEvents.LeavingBackground
        {
            add { _leavingBackground += value; }
            remove { _leavingBackground -= value; }
        }
        TypedEventHandler<PrismApplicationBase, WindowCreatedEventArgs> _windowCreated;
        event TypedEventHandler<PrismApplicationBase, WindowCreatedEventArgs> IPrismApplicationEvents.WindowCreated
        {
            add { _windowCreated += value; }
            remove { _windowCreated -= value; }
        }
    }
}
