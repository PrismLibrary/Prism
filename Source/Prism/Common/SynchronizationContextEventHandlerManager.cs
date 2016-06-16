using System;
using System.Threading;

namespace Prism.Common
{
    /// <summary>
    /// Handles the management and dispatching of EventHandlers using the SynchronizationContext
    /// </summary>
    public static class SynchronizationContextEventHandlerManager
    {
        private static readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public static SynchronizationContext SynchronizationContext { get { return _synchronizationContext; } }

        public static void CallHandler(object sender, EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
                    _synchronizationContext.Post((o) => eventHandler.Invoke(sender, EventArgs.Empty), null);
                else
                    eventHandler.Invoke(sender, EventArgs.Empty);
            }
        }
    }
}
