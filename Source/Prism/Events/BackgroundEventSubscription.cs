using System;
using System.Threading.Tasks;

namespace Prism.Events
{
    /// <summary>
    /// Extends <see cref="EventSubscription"/> to invoke the <see cref="EventSubscription.Action"/> delegate in a background thread.
    /// </summary>
    public class BackgroundEventSubscription : EventSubscription
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundEventSubscription"/>.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="System.Action"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="System.Action"/>.</exception>
        public BackgroundEventSubscription(IDelegateReference actionReference)
            : base(actionReference)
        {
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action"/> in an asynchronous thread by using a <see cref="Task"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public override void InvokeAction(Action action)
        {
            Task.Run(action);
        }
    }

    /// <summary>
    /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate in a background thread.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="System.Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class BackgroundEventSubscription<TPayload> : EventSubscription<TPayload>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundEventSubscription{TPayload}"/>.
        /// </summary>
        /// <param name="actionReference">A reference to a delegate of type <see cref="System.Action{TPayload}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="System.Action{TPayload}"/>,
        /// or the target of <paramref name="filterReference"/> is not of type <see cref="Predicate{TPayload}"/>.</exception>
        public BackgroundEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
            : base(actionReference, filterReference)
        {
        }

        /// <summary>
        /// Invokes the specified <see cref="System.Action{TPayload}"/> in an asynchronous thread by using a <see cref="ThreadPool"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            //ThreadPool.QueueUserWorkItem( (o) => action(argument) );
            Task.Run(() => action(argument));
        }
    }
}