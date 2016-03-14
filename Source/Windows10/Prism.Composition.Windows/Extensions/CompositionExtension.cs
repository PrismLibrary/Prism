namespace Prism.Composition.Windows.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Composition.Hosting.Core;
    using System.Reflection;
    using Adapters;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using Policies;
    using Providers;
    using Strategies;

    /// <summary>
    /// Represents a Unity extension that adds integration with
    /// Managed Extensibility Framework.
    /// </summary>
    public class CompositionExtension : UnityContainerExtension, IDisposable
    {
        /// <summary>
        /// Gets or (sets) the <see cref="System.Composition.Hosting.CompositionHost"/>
        /// </summary>
        public CompositionHost CompositionHostWithDefaultProviders { get; private set; }

        /// <summary>
        /// Gets or (sets) the <see cref="System.Composition.Hosting.CompositionHost"/>
        /// </summary>
        public CompositionHost CompositionHostWithCustomProviders { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="UnityContainerExportDescriptorProvider"/>
        /// </summary>
        private UnityContainerExportDescriptorProvider UnityContainerExportDescriptorProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MefAndUnityContainerExportDescriptorProvider"/>
        /// </summary>
        private MefOrUnityContainerExportDescriptorProvider MefAndUnityContainerExportDescriptorProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UnityContainerAdapter"/>
        /// </summary>
        private UnityContainerAdapter UnityContainerAdapter { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Assemblies"/>
        /// </summary>
        private HashSet<Assembly> Assemblies { get; set; } = new HashSet<Assembly>();

        /// <summary>
        /// Gets or sets the <see cref="Providers"/>
        /// </summary>
        private HashSet<ExportDescriptorProvider> Providers { get; set; } = new HashSet<ExportDescriptorProvider>();

        /// <summary>
        /// Evaluates if a specified type was registered in the container.
        /// </summary>
        /// <param name="type">The type to check if it was registered.</param>
        /// <returns><see langword="true" /> if the <paramref name="type"/> was registered with the container.</returns>
        public bool IsTypeRegistered(Type type)
        {
            return this.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type)) != null;
        }

        /// <summary>
        /// Add a <see cref="Assembly"/> to the <see cref="Assemblies"/> list.
        /// </summary>
        /// <param name="assembly">The assembly to add</param>
        public void WithAssembly(Assembly assembly)
        {
            this.Assemblies.Add(assembly);

            this.UpdateCompositionHost();
        }

        /// <summary>
        /// Add a <see cref="ICollection{Assembly}"/> to the <see cref="Assemblies"/> list.
        /// </summary>
        /// <param name="assemblies">The assemblies to add</param>
        public void WithAssemblies(ICollection<Assembly> assemblies)
        {
            assemblies.ForEach((assembly) =>
            {
                this.Assemblies.Add(assembly);
            });

            this.UpdateCompositionHost();
        }

        /// <summary>
        /// Add a <see cref="ExportDescriptorProvider"/> to the <see cref="Providers"/> list.
        /// </summary>
        /// <param name="provider">The provider to add</param>
        public void WithProvider(ExportDescriptorProvider provider)
        {
            this.Providers.Add(provider);

            this.UpdateCompositionHost();
        }

        /// <summary>
        /// Add a <see cref="ICollection{ExportDescriptorProvider}"/> to the <see cref="Providers"/> list.
        /// </summary>
        /// <param name="providers">The providers to add</param>
        public void WithProviders(ICollection<ExportDescriptorProvider> providers)
        {
            providers.ForEach((provider) =>
            {
                this.Providers.Add(provider);
            });

            this.UpdateCompositionHost();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.CompositionHostWithDefaultProviders != null)
            {
                this.CompositionHostWithDefaultProviders.Dispose();
            }

            if (this.CompositionHostWithCustomProviders != null)
            {
                this.CompositionHostWithCustomProviders.Dispose();
            }

            this.CompositionHostWithDefaultProviders = null;

            this.CompositionHostWithCustomProviders = null;

            this.Providers = null;

            this.Assemblies = null;

            this.UnityContainerExportDescriptorProvider = null;

            this.MefAndUnityContainerExportDescriptorProvider = null;

            this.UnityContainerAdapter = null;
        }

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="ExtensionContext"/> by adding strategies, policies, etc.
        /// to install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            TypeRegistrationTrackerExtension.RegisterIfMissing(this.Container);

            this.UnityContainerAdapter = new UnityContainerAdapter(this.Container);

            this.UnityContainerExportDescriptorProvider = new UnityContainerExportDescriptorProvider(this.UnityContainerAdapter);

            this.MefAndUnityContainerExportDescriptorProvider = new MefOrUnityContainerExportDescriptorProvider(this.UnityContainerAdapter);

            this.CreateCompositionHosts();

            this.Context.Strategies.AddNew<EnumerableResolutionStrategy>(UnityBuildStage.TypeMapping);

            this.Context.Strategies.AddNew<CompositionStrategy>(UnityBuildStage.TypeMapping);

            this.Context.Strategies.AddNew<ComposeStrategy>(UnityBuildStage.Initialization);

            this.Context.Policies.Set<IBuildPlanPolicy>(
                new LazyResolveBuildPlanPolicy(), typeof(Lazy<>));

            this.Context.Policies.SetDefault<ICompositionContainerPolicy>(
                new CompositionContainerPolicy(this.CompositionHostWithCustomProviders, this.CompositionHostWithDefaultProviders));

            this.Container.RegisterInstance<CompositionHost>(this.CompositionHostWithCustomProviders);

            this.Container.RegisterInstance<IUnityContainer>(this.Container);
        }

        /// <summary>
        /// Creates a CompositionHost with default providers and
        /// a CompositionHost with custom providers
        /// </summary>
        private void CreateCompositionHosts()
        {
            var customProviderList = new List<ExportDescriptorProvider>(this.Providers);

            var containerConfiguration = new ContainerConfiguration();

            containerConfiguration.WithAssemblies(this.Assemblies);

            this.CompositionHostWithDefaultProviders = containerConfiguration.CreateContainer();

            containerConfiguration.WithProvider(this.UnityContainerExportDescriptorProvider);

            if (customProviderList.Count > 0)
            {
                customProviderList.ForEach((provider) =>
                {
                    containerConfiguration.WithProvider(provider);
                });
            }

            var compositionHostWithCustomProviders = containerConfiguration.CreateContainer();

            this.MefAndUnityContainerExportDescriptorProvider.CompositionHost = compositionHostWithCustomProviders;

            var mefAndUnityContainerConfiguration = new ContainerConfiguration();

            mefAndUnityContainerConfiguration.WithProvider(this.MefAndUnityContainerExportDescriptorProvider);

            this.CompositionHostWithCustomProviders = mefAndUnityContainerConfiguration.CreateContainer();
        }

        /// <summary>
        /// Creates new CompositionHosts, clears the ICompositionContainerPolicy policy
        /// and sets it again with new values.
        /// </summary>
        private void UpdateCompositionHost()
        {
            this.CreateCompositionHosts();

            this.Context.Policies.ClearDefault<ICompositionContainerPolicy>();

            this.Context.Policies.SetDefault<ICompositionContainerPolicy>(
                new CompositionContainerPolicy(this.CompositionHostWithCustomProviders, this.CompositionHostWithDefaultProviders));

            this.Container.RegisterInstance<CompositionHost>(this.CompositionHostWithCustomProviders);
        }
    }
}
