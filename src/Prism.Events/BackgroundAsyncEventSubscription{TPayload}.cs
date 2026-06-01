using System;
using System.Threading.Tasks;

namespace Prism.Events
{
    /// <summary>
    /// Extends <see cref="AsyncEventSubscription{TPayload}"/> to invoke the delegate on a background thread.
    /// </summary>
    /// <typeparam name="TPayload">The event payload type.</typeparam>
    public class BackgroundAsyncEventSubscription<TPayload> : AsyncEventSubscription<TPayload>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundAsyncEventSubscription{TPayload}"/>.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="Func{TPayload, Task}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        public BackgroundAsyncEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
            : base(actionReference, filterReference)
        {
        }

        /// <inheritdoc/>
        protected override Task InvokeActionAsync(Func<TPayload, Task> action, TPayload argument)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return Task.Run(() => action(argument));
        }
    }
}
