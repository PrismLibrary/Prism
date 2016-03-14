namespace Prism.Composition.Windows.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Reflection;
    using Adapters;

    /// <summary>
    /// Represents an <see cref="ExportDescriptorProvider"/> which can provide
    /// MEF with parts registered in <see cref="IUnityContainer"/>.
    /// </summary>
    public class UnityContainerExportDescriptorProvider : ExportDescriptorProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityContainerExportDescriptorProvider"/> class.
        /// </summary>
        /// <param name="containerAdapter"><see cref="UnityContainerAdapter"/> instance.</param>
        public UnityContainerExportDescriptorProvider(UnityContainerAdapter containerAdapter)
        {
            if (containerAdapter == null)
            {
                throw new ArgumentNullException("containerAdapter");
            }

            this.ContainerAdapter = containerAdapter;

            this.ContainerAdapter.RegisteringComponent += this.OnRegisteringComponentHandler;

            this.ContainerAdapter.Initialize();
        }

        /// <summary>
        /// Gets or sets the <see cref="List{Tuple{Type, string}"/> of known types in the <see cref="IUnityContainer"/>.
        /// </summary>
        private List<Tuple<Type, string>> KnownTypes { get; set; } = new List<Tuple<Type, string>>();

        /// <summary>
        /// Gets or sets the <see cref="ContainerAdapter"/>
        /// </summary>
        private UnityContainerAdapter ContainerAdapter { get; set; }

        /// <summary>
        /// Promises export descriptors for the specified export key.
        /// </summary>
        /// <param name="contract"><see cref="CompositionContract"/> instance.</param>
        /// <param name="descriptorAccessor"><see cref="DependencyAccessor"/> instance.</param>
        /// <returns>A collection of promises for new export descriptors.</returns>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("contract");
            }

            if (descriptorAccessor == null)
            {
                throw new ArgumentNullException("descriptorAccessor");
            }

            var implementations = (IList)descriptorAccessor.ResolveDependencies("default", contract, true);
            
            if (implementations.Count > 0 || !this.KnownTypes.Exists((type) => (type.Item1 == contract.ContractType && type.Item2 == contract.ContractName)))
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }
            else
            {
                return new[] 
                {
                    new ExportDescriptorPromise(
                        contract, 
                        "Unity", 
                        false, 
                        NoDependencies,
                        _ => ExportDescriptor.Create((c, o) => this.ContainerAdapter.Resolve(contract.ContractType, contract.ContractName), NoMetadata))
                };
            }
        }

        /// <summary>
        /// The action handling the event
        /// </summary>
        /// <param name="sender">Sender instance</param>
        /// <param name="e"><see cref="RegisterComponentEventArgs"/> instance.</param>
        private void OnRegisteringComponentHandler(object sender, RegisterComponentEventArgs e)
        {
            if (e.Type.GetTypeInfo().IsInterface)
            {
                this.KnownTypes.Add(Tuple.Create(e.Type, e.Name));
            }
        }
    }
}
