

using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Prism.Unity
{
    /// <summary>
    /// Implements a <see cref="UnityContainerExtension"/> that checks if a specific type was registered with the container.
    /// </summary>
    public class UnityBootstrapperExtension : UnityContainerExtension
    {
        /// <summary>
        /// Evaluates if a specified type was registered in the container.
        /// </summary>
        /// <param name="container">The container to check if the type was registered in.</param>
        /// <param name="type">The type to check if it was registered.</param>
        /// <returns><see langword="true" /> if the <paramref name="type"/> was registered with the container.</returns>
        /// <remarks>
        /// In order to use this extension, you must first call <see cref="UnityExtensions.AddNewExtension{TExtension}"/> 
        /// and specify <see cref="UnityContainerExtension"/> as the extension type.
        /// </remarks>
        public static bool IsTypeRegistered(IUnityContainer container, Type type)
        {
            UnityBootstrapperExtension extension = container.Configure<UnityBootstrapperExtension>();
            if (extension == null)
            {
                //Extension was not added to the container.
                return false;
            }
            IBuildKeyMappingPolicy policy = extension.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type));
            return policy != null;
        }

        ///<summary>
        ///Initializes the container with this extension's functionality.
        ///</summary>
        protected override void Initialize()
        {
        }
    }
}