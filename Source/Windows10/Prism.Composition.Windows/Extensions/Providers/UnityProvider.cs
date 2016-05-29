namespace Prism.Composition.Windows.Extensions.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.Unity;
    
    /// <summary> An unity provider.</summary>
    public class UnityProvider : ExportDescriptorProvider
    {
        /// <summary>Initializes a new instance of the <see cref="UnityProvider"/> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="unityContainer">The unity container.</param>
        public UnityProvider(IUnityContainer unityContainer)
        {
            if (unityContainer == null)
            {
                throw new ArgumentNullException("unityContainer");
            }

            this.UnityContainer = unityContainer;
        }
        
        /// <summary>Gets or sets the unity container.</summary>
        /// <value>The unity container.</value>
        private IUnityContainer UnityContainer { get; set; }
        
        /// <summary>Promise export descriptors for the specified export key.</summary>
        /// <param name="contract">The export key required by another component.</param>
        /// <param name="descriptorAccessor">Accesses the other export descriptors present in the composition.</param>
        /// <returns>Promises for new export descriptors.</returns>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            var type = contract.ContractType;

            if (!this.UnityContainer.IsTypeRegistered(type))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            var containerRegistrations = this.UnityContainer.Registrations.Where(t => t.RegisteredType == type);

            if (type.GetTypeInfo().IsGenericType)
            {
                containerRegistrations = containerRegistrations.Concat(this.UnityContainer.Registrations.Where(t => t.RegisteredType == type.GetGenericTypeDefinition()));
            }

            if (!string.IsNullOrEmpty(contract.ContractName))
            {
                var specificContainerRegistrations = this.UnityContainer.Registrations.Where(t => t.RegisteredType == type && t.Name == contract.ContractName);

                if (type.GetTypeInfo().IsGenericType)
                {
                    specificContainerRegistrations = specificContainerRegistrations.Concat(this.UnityContainer.Registrations.Where(t => t.RegisteredType == type.GetGenericTypeDefinition() && t.Name == contract.ContractName));
                }

                if (specificContainerRegistrations.Count() > 0)
                {
                    containerRegistrations = new List<ContainerRegistration>() { specificContainerRegistrations.First() };
                }
                else
                {
                    containerRegistrations = new List<ContainerRegistration>() { containerRegistrations.First() };
                }
            }

            var exportDescriptorPromiseList = new List<ExportDescriptorPromise>();

            foreach (var containerRegistration in containerRegistrations)
            {
                var exportDescriptorPromise = new ExportDescriptorPromise(
                        contract,
                        "Unity Container",
                        true,
                        NoDependencies,
                        _ => ExportDescriptor.Create((c, o) => this.UnityContainer.Resolve(type, containerRegistration.Name), NoMetadata));

                exportDescriptorPromiseList.Add(exportDescriptorPromise);
            }

            return exportDescriptorPromiseList;
        }
    }
}
