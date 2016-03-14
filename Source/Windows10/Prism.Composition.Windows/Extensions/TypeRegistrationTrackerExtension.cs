namespace Prism.Composition.Windows.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Unity extension that exposes events which can be used
    /// to track types registered within <see cref="IUnityContainer"/> container.
    /// </summary>
    public sealed class TypeRegistrationTrackerExtension : UnityContainerExtension
    {
        /// <summary>
        /// Event raised whenever an instance is being registered.
        /// </summary>
        public event EventHandler<RegisterInstanceEventArgs> RegisteringInstanceEventHandler;

        /// <summary>
        /// Event raised whenever a type is being registered.
        /// </summary>
        public event EventHandler<RegisterEventArgs> RegisteringEventHandler;

        /// <summary>
        /// Gets all types registered in the <see cref="IUnityContainer"/> since
        /// this extension was enabled.
        /// </summary>
        public ReadOnlyCollection<TypeRegistrationEntry> Entries
        {
            get { return this.TypeRegistrationEntryList.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets the <see cref="List{TypeRegistrationEntry}"/>
        /// </summary>
        private List<TypeRegistrationEntry> TypeRegistrationEntryList { get; set; } = new List<TypeRegistrationEntry>();

        /// <summary>
        /// Register the <see cref="TypeRegistrationTrackerExtension"/> in the <see cref="IUnityContainer"/>
        /// </summary>
        /// <param name="unityContainer"><see cref="IUnityContainer"/> instance.</param>
        public static void RegisterIfMissing(IUnityContainer unityContainer)
        {
            var typeRegistrationTrackerExtension = unityContainer.Configure<TypeRegistrationTrackerExtension>();

            if (typeRegistrationTrackerExtension == null)
            {
                unityContainer.AddNewExtension<TypeRegistrationTrackerExtension>();
            }
        }

        /// <summary>
        /// Removes the extension's functions from the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called when extensions are being removed from the container. It can be
        /// used to do things like disconnect event handlers or clean up member state. You do not
        /// need to remove strategies or policies here; the container will do that automatically.
        /// </para>
        /// <para>
        /// The default implementation of this method does nothing.
        /// </para>
        /// </remarks>
        public override void Remove()
        {
            this.Context.Registering -= this.OnRegistering;
            this.Context.RegisteringInstance -= this.OnRegisteringInstance;
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
            this.Context.Registering += this.OnRegistering;
            this.Context.RegisteringInstance += this.OnRegisteringInstance;
        }

        /// <summary>
        /// The action for <see cref="EventHandler{RegisterInstanceEventArgs}"/>
        /// </summary>
        /// <param name="s">The sender</param>
        /// <param name="e">The Event</param>
        private void OnRegisteringInstance(object s, RegisterInstanceEventArgs e)
        {
            this.TypeRegistrationEntryList.Add(new TypeRegistrationEntry(e.RegisteredType, e.Name));

            if (this.RegisteringInstanceEventHandler != null)
            {
                this.RegisteringInstanceEventHandler(s, e);
            }
        }

        /// <summary>
        /// The action for <see cref="EventHandler{RegisterEventArgs}"/>
        /// </summary>
        /// <param name="s">The sender</param>
        /// <param name="e">The Event</param>
        private void OnRegistering(object s, RegisterEventArgs e)
        {
            this.TypeRegistrationEntryList.Add(new TypeRegistrationEntry(e.TypeFrom ?? e.TypeTo, e.Name));

            if (this.RegisteringEventHandler != null)
            {
                this.RegisteringEventHandler(s, e);
            }
        }
    }
}
