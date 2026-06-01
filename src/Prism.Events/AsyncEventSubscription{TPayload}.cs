using System;
using System.Globalization;
using System.Threading.Tasks;
using Prism.Events.Properties;

namespace Prism.Events
{
    /// <summary>
    /// Provides a way to retrieve an asynchronous delegate to execute depending on a filter predicate.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic payload and filter types.</typeparam>
    public class AsyncEventSubscription<TPayload> : IAsyncEventSubscription
    {
        private readonly IDelegateReference _actionReference;
        private readonly IDelegateReference _filterReference;

        /// <summary>
        /// Creates a new instance of <see cref="AsyncEventSubscription{TPayload}"/>.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="Func{TPayload, Task}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <paramref name="filterReference"/> are <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">When <paramref name="actionReference"/> or <paramref name="filterReference"/> target the wrong delegate type.</exception>
        public AsyncEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
        {
            if (actionReference == null)
                throw new ArgumentNullException(nameof(actionReference));
            if (!(actionReference.Target is Func<TPayload, Task>))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidDelegateRerefenceTypeException, typeof(Func<TPayload, Task>).FullName), nameof(actionReference));

            if (filterReference == null)
                throw new ArgumentNullException(nameof(filterReference));
            if (!(filterReference.Target is Predicate<TPayload>))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidDelegateRerefenceTypeException, typeof(Predicate<TPayload>).FullName), nameof(filterReference));

            _actionReference = actionReference;
            _filterReference = filterReference;
        }

        /// <summary>
        /// Gets the target <see cref="Func{TPayload, Task}"/> referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        public Func<TPayload, Task> Action => (Func<TPayload, Task>)_actionReference.Target;

        /// <summary>
        /// Gets the target <see cref="Predicate{TPayload}"/> referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        public Predicate<TPayload> Filter => (Predicate<TPayload>)_filterReference.Target;

        /// <summary>
        /// Gets or sets the token that identifies this subscription.
        /// </summary>
        public SubscriptionToken SubscriptionToken { get; set; }

        /// <inheritdoc/>
        public virtual Action<object[]> GetExecutionStrategy()
        {
            var action = Action;
            var filter = Filter;
            if (action != null && filter != null)
                return _ => { };

            return null;
        }

        /// <inheritdoc/>
        public virtual Func<object[], Task> GetAsyncExecutionStrategy()
        {
            var action = Action;
            var filter = Filter;
            if (action != null && filter != null)
            {
                return arguments =>
                {
                    TPayload argument = default(TPayload);
                    if (arguments != null && arguments.Length > 0 && arguments[0] != null)
                    {
                        argument = (TPayload)arguments[0];
                    }

                    return filter(argument)
                        ? InvokeActionAsync(action, argument)
                        : Task.CompletedTask;
                };
            }

            return null;
        }

        /// <summary>
        /// Invokes the specified action asynchronously when not overridden.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass to <paramref name="action"/>.</param>
        protected virtual Task InvokeActionAsync(Func<TPayload, Task> action, TPayload argument)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return action(argument) ?? Task.CompletedTask;
        }
    }
}
