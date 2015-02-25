// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Prism.Commands
{
    /// <summary>
    /// Handles management and dispatching of EventHandlers in a weak way.
    /// </summary>
    public static class WeakEventHandlerManager
    {
        private static readonly SynchronizationContext syncContext = SynchronizationContext.Current;

        ///<summary>
        /// Invokes the handlers 
        ///</summary>
        ///<param name="sender"></param>
        ///<param name="handlers"></param>
        public static void CallWeakReferenceHandlers(object sender, List<WeakReference> handlers)
        {
            if (handlers != null)
            {
                // Take a snapshot of the handlers before we call out to them since the handlers
                // could cause the array to me modified while we are reading it.
                EventHandler[] callees = new EventHandler[handlers.Count];
                int count = 0;

                //Clean up handlers
                count = CleanupOldHandlers(handlers, callees, count);

                // Call the handlers that we snapshotted
                for (int i = 0; i < count; i++)
                {
                    CallHandler(sender, callees[i]);
                }
            }
        }

        private static void CallHandler(object sender, EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                if (syncContext != null)
                {
                    syncContext.Post((o) => eventHandler(sender,  EventArgs.Empty), null);
                }
                else
                {
                    eventHandler(sender, EventArgs.Empty);
                }
            }
        }

        private static int CleanupOldHandlers(List<WeakReference> handlers, EventHandler[] callees, int count)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                WeakReference reference = handlers[i];
                EventHandler handler = reference.Target as EventHandler;
                if (handler == null)
                {
                    // Clean up old handlers that have been collected
                    handlers.RemoveAt(i);
                }
                else
                {
                    callees[count] = handler;
                    count++;
                }
            }
            return count;
        }

        ///<summary>
        /// Adds a handler to the supplied list in a weak way.
        ///</summary>
        ///<param name="handlers">Existing handler list.  It will be created if null.</param>
        ///<param name="handler">Handler to add.</param>
        ///<param name="defaultListSize">Default list size.</param>
        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (handlers == null)
            {
                handlers = (defaultListSize > 0 ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
            }

            handlers.Add(new WeakReference(handler));
        }

        ///<summary>
        /// Removes an event handler from the reference list.
        ///</summary>
        ///<param name="handlers">Handler list to remove reference from.</param>
        ///<param name="handler">Handler to remove.</param>
        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers != null)
            {
                for (int i = handlers.Count - 1; i >= 0; i--)
                {
                    WeakReference reference = handlers[i];
                    EventHandler existingHandler = reference.Target as EventHandler;
                    if ((existingHandler == null) || (existingHandler == handler))
                    {
                        // Clean up old handlers that have been collected
                        // in addition to the handler that is to be removed.
                        handlers.RemoveAt(i);
                    }
                }
            }
        }
    }
}
