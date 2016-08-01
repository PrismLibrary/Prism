

using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Common;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Implementation of the <see cref="IInteractionRequest"/> interface.
    /// </summary>
    public class InteractionRequest<T> : IInteractionRequest
        where T : INotification
    {
        /// <summary>
        /// Fired when interaction is needed.
        /// </summary>
        public event EventHandler<InteractionRequestedEventArgs> Raised;

        /// <summary>
        /// Fires the Raised event.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void Raise(T context)
        {
            this.Raise(context, c => { });
        }

        /// <summary>
        /// Fires the Raised event.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        /// <param name="callback">The callback to execute when the interaction is completed.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void Raise(T context, Action<T> callback)
        {
            var handler = this.Raised;
            if (handler != null)
            {
                handler(this, new InteractionRequestedEventArgs(context, () => { if(callback != null) callback(context); } ));
            }
        }

        /// <summary>
        /// Fires the Raised event asynchronously. Please note that this request may never return
        /// if the InteractionRequest is unhandled.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        /// <returns>
        /// The context after the request has been handled by the UI.
        /// </returns>
        public Task<T> RaiseAsync(T context)
        {
            return this.RaiseAsync(context, false);
        }

        /// <summary>
        /// Fires the Raised event asynchronously. Please note that this request may never return
        /// if the InteractionRequest is unhandled.
        /// </summary>
        /// <param name="context">The context for the interaction request.</param>
        /// <param name="executeSynchronously">
        /// Has no effect if the popup window is not modal.
        /// If the popup window is modal and executeSynchronously is true, returns already completed task.
        /// That is, when the method returns, the popup window has already been closed.
        /// </param>
        /// <returns>
        /// The context after the request has been handled by the UI.
        /// </returns>
        public Task<T> RaiseAsync(T context, bool executeSynchronously)
        {
            if (executeSynchronously)
            {
                return CallbackHelper.AwaitCallbackResult<T>(callback => this.Raise(context, callback));
            }
            else
            {
                return this.RaiseImplAsync(context);
            }
        }

        private async Task<T> RaiseImplAsync(T context)
        {
            // SynchronizationContext exists in actual program running on GUI Dispatcher thread.  Make sure
            // it exists in tests.
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            }

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            await Task.Factory.StartNew(() =>
            {
                this.Raise(context, (c) => tcs.TrySetResult(c));
            },
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext());

            return await tcs.Task;
        }
    }
}
