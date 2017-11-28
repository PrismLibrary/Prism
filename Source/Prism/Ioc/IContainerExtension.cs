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
        /// Determines if the container can be used with modules.
        /// </summary>
        /// <remarks>Only containers that are mutable can support modules.</remarks>
        bool SupportsModules { get; }

        /// <summary>
        /// Used to perform any final steps for configuring the extension that may be required by the container.
        /// </summary>
        void FinalizeExtension();

        /// <summary>
        /// Used as the ViewModel resolver for ViewModelLocationProvider.SetDefaultViewModelFactory
        /// </summary>
        /// <param name="view">The view instance</param>
        /// <param name="viewModelType">The ViewModel type to create</param>
        /// <returns></returns>
        object ResolveViewModelForView(object view, Type viewModelType);
    }
}
