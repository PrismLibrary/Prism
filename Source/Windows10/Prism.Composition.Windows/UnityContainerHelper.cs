namespace Prism.Composition.Windows
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using Microsoft.Practices.Unity;
    
    /// <summary>An unity container helper.</summary>
    public static class UnityContainerHelper
    {
        /// <summary>An IUnityContainer extension method that queries if a type is registered.</summary>
        /// <param name="container">The container to act on. </param>
        /// <param name="type">     The type. </param>
        /// <returns>true if the type is registered, false if not.</returns>
        public static bool IsTypeRegistered(this IUnityContainer container, Type type)
        {
            return CompositionExtension.IsTypeRegistered(container, type);
        }
        
        /// <summary>An IUnityContainer extension method that try resolve.</summary>
        /// <typeparam name="T">Generic type parameter. </typeparam>
        /// <param name="container">The container to act on. </param>
        /// <returns>An object.</returns>
        public static T TryResolve<T>(this IUnityContainer container)
        {
            object result = TryResolve(container, typeof(T));

            if (result != null)
            {
                return (T)result;
            }

            return default(T);
        }
        
        /// <summary>An IUnityContainer extension method that try resolve.</summary>
        /// <param name="container">    The container to act on. </param>
        /// <param name="typeToResolve">The type to resolve. </param>
        /// <returns>An object.</returns>
        public static object TryResolve(this IUnityContainer container, Type typeToResolve)
        {
            try
            {
                return container.Resolve(typeToResolve);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>An IUnityContainer extension method that registers the assembly configuration.</summary>
        /// <param name="unityContainer">       The unityContainer to act on. </param>
        /// <param name="assemblyConfiguration">The assembly configuration. </param>
        public static void RegisterAssemblyConfiguration(this IUnityContainer unityContainer, AssemblyConfiguration assemblyConfiguration)
        {
            lock (unityContainer)
            {
                var compositionIntegration = EnableCompositionExtension(unityContainer);

                compositionIntegration.AssemblyConfigurationList.Add(assemblyConfiguration);
            }
        }
        
        /// <summary>An IUnityContainer extension method that registers the assembly configuration.</summary>
        /// <param name="unityContainer">        The unityContainer to act on. </param>
        /// <param name="assemblyConfigurations">The assembly configurations. </param>
        public static void RegisterAssemblyConfiguration(this IUnityContainer unityContainer, IEnumerable<AssemblyConfiguration> assemblyConfigurations)
        {
            lock (unityContainer)
            {
                var compositionIntegration = EnableCompositionExtension(unityContainer);

                foreach (var assemblyConfiguration in assemblyConfigurations)
                {
                    compositionIntegration.AssemblyConfigurationList.Add(assemblyConfiguration);
                }
            }
        }
        
        /// <summary>An IUnityContainer extension method that enables the composition extension.</summary>
        /// <param name="unityContainer">The unityContainer to act on. </param>
        /// <returns>A CompositionExtension.</returns>
        public static CompositionExtension EnableCompositionExtension(this IUnityContainer unityContainer)
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
    }
}