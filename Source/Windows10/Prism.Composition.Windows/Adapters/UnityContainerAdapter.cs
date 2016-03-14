namespace Prism.Composition.Windows.Adapters
{
    using System;
    using Extensions;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Represents an adapter for the <see cref="IUnityContainer"/> container.
    /// </summary>
    public class UnityContainerAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityContainerAdapter"/> class.
        /// </summary>
        /// <param name="unityContainer"><see cref="IUnityContainer"/> instance.</param>
        public UnityContainerAdapter(IUnityContainer unityContainer)
        {
            if (unityContainer == null)
            {
                throw new ArgumentNullException("unityContainer");
            }

            this.UnityContainer = unityContainer;
        }

        /// <summary>
        /// Event raised whenever a component gets registered in
        /// the underlying container.
        /// </summary>
        public event EventHandler<RegisterComponentEventArgs> RegisteringComponent;

        /// <summary>
        /// Gets or sets the <see cref="IUnityContainer"/>
        /// </summary>
        private IUnityContainer UnityContainer { get; set; }

        /// <summary>
        /// Method called by <see cref="ContainerExportDescriptorProvider"/> to retrieve
        /// an instance of a given type.
        /// </summary>
        /// <param name="type">Type of the instance to retrieve.</param>
        /// <param name="name">Optional name.</param>
        /// <returns>An instance of a given type.</returns>
        public object Resolve(Type type, string name)
        {
            return this.UnityContainer.Resolve(type, name);
        }

        /// <summary>
        /// Method called by <see cref="ContainerExportDescriptorProvider"/> in order
        /// to initialize the adapter.
        /// </summary>
        public void Initialize()
        {
            TypeRegistrationTrackerExtension.RegisterIfMissing(this.UnityContainer);

            var tracker = this.UnityContainer.Configure<TypeRegistrationTrackerExtension>();

            foreach (var entry in tracker.Entries)
            {
                this.OnRegisteringComponent(entry.Type, entry.Name);
            }

            tracker.RegisteringEventHandler += (sender, eventArguments) =>
                this.OnRegisteringComponent(eventArguments.TypeFrom ?? eventArguments.TypeTo, eventArguments.Name);

            tracker.RegisteringInstanceEventHandler += (sender, eventArguments) =>
                this.OnRegisteringComponent(eventArguments.RegisteredType, eventArguments.Name);
        }

        /// <summary>
        /// Fires <see cref="RegisteringComponent"/> event.
        /// </summary>
        /// <param name="type">Type being registered.</param>
        /// <param name="name">Optional name.</param>
        protected void OnRegisteringComponent(Type type, string name)
        {
            if (this.RegisteringComponent != null)
            {
                this.RegisteringComponent(this, new RegisterComponentEventArgs(type, name));
            }
        }
    }
}
