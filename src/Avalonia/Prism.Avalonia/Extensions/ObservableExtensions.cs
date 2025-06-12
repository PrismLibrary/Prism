using System;
using Avalonia.Reactive;
using Prism.Common;

namespace Prism.Extensions
{
    internal static class ObservableExtensions
    {
        /// <summary>
        /// Subscribes an element handler to an observable sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Observable sequence to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <returns><see cref="IDisposable"/> object used to unsubscribe from the observable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="onNext"/> is <c>null</c>.</exception>
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (onNext == null)
            {
                throw new ArgumentNullException(nameof(onNext));
            }

            //
            // [OK] Use of unsafe Subscribe: non-pretentious constructor for an observer; this overload is not to be used internally.
            //
            return source.Subscribe/*Unsafe*/(new AnonymousObserver<T>(onNext, Stubs.Throw, Stubs.Nop));
        }
    }
}
