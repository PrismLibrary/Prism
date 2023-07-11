#nullable enable
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
        private static IContainerExtension? _current;

        /// <summary>
        /// Gets the current <see cref="IContainerExtension" />.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IContainerExtension Current =>
            _current ?? throw new InvalidOperationException("The `ContainerLocator.SetContainerExtension` method has not been invoked");

        /// <summary>
        /// Gets the <see cref="IContainerProvider" />
        /// </summary>
        public static IContainerProvider Container =>
            Current;

        /// <summary>
        /// Sets the Container to use if the <see cref="IContainerProvider" /> is null. Otherwise this will throw an exception.
        /// </summary>
        /// <param name="instance">The current instance to set</param>
        /// <remarks>
        /// <exception cref="InvalidOperationException">Throws an exception if the Container has already been set.</exception>
        public static void SetContainerExtension(IContainerExtension instance)
        {
            if (_current is not null)
            {
                throw new InvalidOperationException("The Container has already been initialized with the ContainerLocator. Please use TrySetContainerExtension or Reset the ContainerLocator prior to invoking the SetContainerExtensionMethod.");
            }
            _current = instance;
        }

        /// <summary>
        /// Returns <c>True</c> and sets the Container if the <see cref="IContainerProvider"/> is null. Otherwise
        /// will return <c>False</c>
        /// </summary>
        /// <param name="instance">The instance of the <see cref="IContainerExtension"/> to set.</param>
        /// <returns><c>True</c> if the Container was set.</returns>
        public static bool TrySetContainerExtension(IContainerExtension instance)
        {
            if (_current is not null)
                return false;

            _current = instance;
            return true;
        }

        /// <summary>
        /// Used for Testing to Reset the Current Container
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ResetContainer()
        {
            _current = null;
        }
    }
}
