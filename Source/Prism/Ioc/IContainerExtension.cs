using System;

namespace Prism.Ioc
{
    public interface IContainerExtension<TContainer> : IContainerExtension
    {
        /// <summary>
        /// The instance of the wrapped container
        /// </summary>
        TContainer Instance { get; }
    }

    public interface IContainerExtension : IContainerProvider, IContainerRegistry
    {
        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        void FinalizeExtension();
    }
}
