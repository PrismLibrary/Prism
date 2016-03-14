namespace Prism.Composition.Windows.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using System.Reflection;
    using Prism.Composition.Windows.Adapters;

    /// <summary>
    /// This class delegates between a "pure" Unity provider and a different <see cref="CompositionHost"/>
    /// instance with a different setup in providers.
    /// </summary>
    public class MefOrUnityContainerExportDescriptorProvider : ExportDescriptorProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefOrUnityContainerExportDescriptorProvider"/> class.
        /// </summary>
        /// <param name="containerAdapter"><see cref="UnityContainerAdapter"/> instance.</param>
        public MefOrUnityContainerExportDescriptorProvider(UnityContainerAdapter containerAdapter)
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
        /// Gets or sets the <see cref="CompositionHost"/>.
        /// </summary>
        public CompositionHost CompositionHost { get; set; }

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

            var implementations = (IList)descriptorAccessor.ResolveDependencies("default", contract, false);

            if (implementations.Count > 0 || contract.ContractType.GetTypeInfo().IsGenericType)
            {
                return ExportDescriptorProvider.NoExportDescriptors;
            }

            if (!this.KnownTypes.Exists((t) => (t.Item1 == contract.ContractType && t.Item2 == contract.ContractName)))
            {
                return new[]
                {
                    new ExportDescriptorPromise(
                        contract,
                        "Mef and Unity",
                        false,
                        NoDependencies,
                        _ => ExportDescriptor.Create(
                            (c, o) =>
                            {
                                object export = null;

                                this.CompositionHost.TryGetExport(contract, out export);

                                return export;
                            }, 
                            NoMetadata))
                };
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
                        _ => ExportDescriptor.Create(
                            (c, o) => this.ContainerAdapter.Resolve(contract.ContractType, contract.ContractName), 
                            NoMetadata))
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
