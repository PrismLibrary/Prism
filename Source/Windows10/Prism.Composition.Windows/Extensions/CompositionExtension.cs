namespace Prism.Composition.Windows.Extensions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Composition.Hosting;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;
    using Policies;
    using Providers;
    using Strategies;
    
    /// <summary>A composition extension.</summary>
    public class CompositionExtension : UnityContainerExtension
    {
        /// <summary>Initializes a new instance of the <see cref="CompositionExtension"/> class.</summary>
        public CompositionExtension()
        {
            this.AssemblyConfigurationList.CollectionChanged += this.AssemblyConfigurationListCollectionChanged;
        }

        /// <summary>Initializes a new instance of the <see cref="CompositionExtension"/> class.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="assembly">The assembly.</param>
        public CompositionExtension(AssemblyConfiguration assembly) : this()
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            this.AssemblyConfigurationList.Add(assembly);
        }
        
        /// <summary>Gets a list of assembly configurations.</summary>
        /// <value>A List of assembly configurations.</value>
        public ObservableCollection<AssemblyConfiguration> AssemblyConfigurationList { get; private set; } = new ObservableCollection<AssemblyConfiguration>();
        
        /// <summary>Gets or sets the composition host.</summary>
        /// <value>The composition host.</value>
        private CompositionHost CompositionHost { get; set; }
        
        /// <summary>Queries if a type is registered.</summary>
        /// <param name="container">The container. </param>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is registered, false if not.</returns>
        public static bool IsTypeRegistered(IUnityContainer container, Type type)
        {
            CompositionExtension extension = container.Configure<CompositionExtension>();

            if (extension == null)
            {
                return false;
            }

            IBuildKeyMappingPolicy policy = extension.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type));

            return policy != null;
        }
        
        /// <summary>Initial the container with this extension's functionality.</summary>
        protected override void Initialize()
        {
            this.CompositionHost = this.CreateCompositionHost();

            this.Container.RegisterInstance<CompositionHost>(this.CompositionHost);

            this.Context.Strategies.AddNew<TypeMappingStrategy>(UnityBuildStage.TypeMapping);

            this.Context.Strategies.AddNew<InitializationStrategy>(UnityBuildStage.Initialization);

            CompositionHostPolicy compositionHostPolicy = new CompositionHostPolicy(this.CompositionHost);

            this.Context.Policies.SetDefault(typeof(ICompositionHostPolicy), compositionHostPolicy);
        }
        
        /// <summary>Creates composition host.</summary>
        /// <returns>The new composition host.</returns>
        private CompositionHost CreateCompositionHost()
        {
            var configuration = new ContainerConfiguration();

            foreach (var assemblyConfiguration in this.AssemblyConfigurationList)
            {
                if (assemblyConfiguration.Assembly != null)
                {
                    configuration.WithAssembly(assemblyConfiguration.Assembly, assemblyConfiguration.Conventions);
                }
            }

            configuration.WithProvider(new UnityProvider(this.Container));

            return configuration.CreateContainer();
        }
        
        /// <summary>Assembly configuration list collection changed.</summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">Notify collection changed event information.</param>
        private void AssemblyConfigurationListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.CompositionHost = this.CreateCompositionHost();

            this.Container.RegisterInstance<CompositionHost>(this.CompositionHost);

            this.Context.Policies.SetDefault(typeof(ICompositionHostPolicy), new CompositionHostPolicy(this.CompositionHost));
        }
    }
}