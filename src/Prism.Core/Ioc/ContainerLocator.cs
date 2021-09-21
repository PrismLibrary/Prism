using System;
using System.ComponentModel;

namespace Prism.Ioc
{
    /// <summary>
    /// The <see cref="ContainerLocator" /> tracks the current instance of the Container used by your Application
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ContainerLocator
    {
        private static IContainerExtension _current;

        /// <summary>
        /// Gets the current <see cref="IContainerExtension" />.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IContainerExtension Current =>
            _current;

        /// <summary>
        /// Gets the <see cref="IContainerProvider" />
        /// </summary>
        public static IContainerProvider Container =>
            Current;

        /// <summary>
        /// Sets the Container to use if the Current <see cref="IContainerProvider" /> is null
        /// </summary>
        /// <param name="factory"></param>
        /// <exception cref="InvalidOperationException">Will throw an exception if the Container has not </exception>
        public static void SetContainerExtension(Func<IContainerExtension> factory) =>
            SetContainerExtension(factory());

        /// <summary>
        /// Sets a new instance of the container
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainerExtension(IContainerExtension container)
        {
            if(_current != null)
            {
                throw new InvalidOperationException("The Current container is not null, and cannot be set again. In order to set the container you must first call ResetContainer.");
            }

            _current = container;
        }

        /// <summary>
        /// Used for Testing to Reset the Current Container
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ResetContainer()
        {
            _current?.Dispose();
            _current = null;
        }
    }
}
