using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prism.Events
{
    /// <summary>
    /// Extends <see cref="AsyncEventSubscription{TPayload}"/> to invoke the delegate in a specific <see cref="SynchronizationContext"/>.
    /// </summary>
    /// <typeparam name="TPayload">The event payload type.</typeparam>
    public class DispatcherAsyncEventSubscription<TPayload> : AsyncEventSubscription<TPayload>
    {
        private readonly SynchronizationContext syncContext;

        /// <summary>
        /// Creates a new instance of <see cref="DispatcherAsyncEventSubscription{TPayload}"/>.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="Func{TPayload, Task}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        /// <param name="context">The synchronization context to use for UI thread dispatching.</param>
        public DispatcherAsyncEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference, SynchronizationContext context)
            : base(actionReference, filterReference)
        {
            syncContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        protected override Task InvokeActionAsync(Func<TPayload, Task> action, TPayload argument)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var taskCompletionSource = new TaskCompletionSource<object>();
            syncContext.Post(async _ =>
            {
                try
                {
                    var task = action(argument);
                    if (task != null)
                        await task.ConfigureAwait(false);

                    taskCompletionSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }, null);

            return taskCompletionSource.Task;
        }
    }
}
