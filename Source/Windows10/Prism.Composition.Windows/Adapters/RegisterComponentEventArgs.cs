namespace Prism.Composition.Windows.Adapters
{
    using System;

    /// <summary>
    /// Represents arguments for <see cref="UnityContainerAdapter.RegisteringComponent"/> event.
    /// </summary>
    public class RegisterComponentEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterComponentEventArgs"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> being registered.</param>
        /// <param name="name">Optional name.</param>
        public RegisterComponentEventArgs(Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.Type = type;

            this.Name = name;
        }

        /// <summary>
        /// Gets or (sets) the <see cref="Type"/> being registered.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets or (sets) the optional name used during component registration.
        /// </summary>
        public string Name { get; private set; }
    }
}
