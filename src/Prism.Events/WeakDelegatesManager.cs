using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Events
{
    /// <summary>
    /// Manage delegates using weak references to prevent keeping target instances longer than expected.
    /// </summary>
    public class WeakDelegatesManager
    {
        private readonly List<DelegateReference> _listeners = new List<DelegateReference>();

        /// <summary>
        /// Adds a weak reference to the specified <see cref="Delegate"/> listener.
        /// </summary>
        /// <param name="listener">The original <see cref="Delegate"/> to add.</param>
        public void AddListener(Delegate listener)
        {
            _listeners.Add(new DelegateReference(listener, false));
        }

        /// <summary>
        /// Removes the weak reference to the specified <see cref="Delegate"/> listener.
        /// </summary>
        /// <param name="listener">The original <see cref="Delegate"/> to remove.</param>
        public void RemoveListener(Delegate listener)
        {
            //Remove the listener, and prune collected listeners
            _listeners.RemoveAll(reference => reference.TargetEquals(null) || reference.TargetEquals(listener));
        }

        /// <summary>
        /// Invoke the delegates for all targets still being alive.
        /// </summary>
        /// <param name="args">An array of objects that are the arguments to pass to the delegates. -or- null, if the method represented by the delegate does not require arguments. </param>
        public void Raise(params object[] args)
        {
            _listeners.RemoveAll(listener => listener.TargetEquals(null));

            foreach (Delegate handler in _listeners.Select(listener => listener.Target).Where(listener => listener != null).ToList())
            {
                handler.DynamicInvoke(args);
            }
        }
    }
}
