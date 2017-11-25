namespace Prism.Ioc
{
    public interface IContainerExtension<TContainer> : IContainerProvider<TContainer>, IContainerExtension
    {

    }

    public interface IContainerExtension : IContainerProvider, IContainerRegistry
    {
        /// <summary>
        /// Determines if the container can be used with modules.
        /// </summary>
        /// <remarks>Only containers that are mutable can support modules.</remarks>
        bool SupportsModules { get; }

        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        void FinalizeExtension();
    }
}
