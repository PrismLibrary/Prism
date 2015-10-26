using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Windows.Navigation
{
    public enum StateChangeType
    {
        None, Back, Forward, Clear, Remove, Set
    }

    public class NavigationStateChangedEventArgs
    {
        private WeakReference<IFrameFacade> _frameFacadeWeakRef;

        public NavigationStateChangedEventArgs(IFrameFacade frameFacade, StateChangeType stateChange)
        {
            _frameFacadeWeakRef = new WeakReference<IFrameFacade>(frameFacade);
            StateChange = stateChange;
        }

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

        public StateChangeType StateChange { get; private set; }
    }
}