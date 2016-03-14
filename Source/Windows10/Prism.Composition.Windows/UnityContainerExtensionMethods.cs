namespace Prism.Composition.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Extensions;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Contains extensions for the <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static class UnityContainerExtensionMethods
    {
        /// <summary>
        /// Registers a MEF catalog within Unity container.
        /// </summary>
        /// <param name="unityContainer">Unity container instance.</param>
        /// <param name="assembly">The assembly to be registered.</param>
        public static void RegisterAssembly(this IUnityContainer unityContainer, Assembly assembly)
        {
            lock (unityContainer)
            {
                var compositionIntegration = EnableCompositionIntegration(unityContainer);

                compositionIntegration.WithAssembly(assembly);
            }
        }

        /// <summary>
        /// Registers a MEF catalog within Unity container.
        /// </summary>
        /// <param name="unityContainer">Unity container instance.</param>
        /// <param name="assemblyList">The assemblies to be registered.</param>
        public static void RegisterAssemblies(this IUnityContainer unityContainer, List<Assembly> assemblyList)
        {
            lock (unityContainer)
            {
                var compositionIntegration = EnableCompositionIntegration(unityContainer);

                compositionIntegration.WithAssemblies(assemblyList);
            }
        }

        /// <summary>
        /// Enables Managed Extensibility Framework integration.
        /// </summary>
        /// <param name="unityContainer">Target container.</param>
        /// <returns><see cref="CompositionExtension"/> instance.</returns>
        public static CompositionExtension EnableCompositionIntegration(this IUnityContainer unityContainer)
        {
            lock (unityContainer)
            {
                var compositionExtension = unityContainer.Configure<CompositionExtension>();

                if (compositionExtension == null)
                {
                    compositionExtension = new CompositionExtension();

                    unityContainer.AddExtension(compositionExtension);
                }

                return compositionExtension;
            }
        }

        /// <summary>
        /// Evaluates if a specified type was registered in the container.
        /// </summary>
        /// <param name="unityContainer">The container to check if the type was registered in.</param>
        /// <param name="type">The type to check if it was registered.</param>
        /// <returns><see langword="true" /> if the <paramref name="type"/> was registered with the container.</returns>
        public static bool IsTypeRegistered(this IUnityContainer unityContainer, Type type)
        {
            var compositionExtension = unityContainer.Configure<CompositionExtension>();

            if (compositionExtension == null)
            {
                return false;
            }
            
            return compositionExtension.IsTypeRegistered(type);
        }
    }
}
