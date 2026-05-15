using System;

namespace Prism
{
    /// <summary>
    /// Interface that defines if the object instance is active
    /// and notifies when the activity changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IActiveAware"/> interface is used to track the active state of objects within Prism,
    /// particularly in regions. When a view or module becomes active or inactive, implementations of this interface
    /// can respond to the state change through the <see cref="IsActiveChanged"/> event.
    /// </para>
    /// <para>
    /// This is commonly used for ViewModels that need to load or unload data based on whether their corresponding
    /// view is currently being displayed in a region.
    /// </para>
    /// </remarks>
    public interface IActiveAware
    {
        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value><see langword="true" /> if the object is active; otherwise <see langword="false" />.</value>
        /// <remarks>
        /// When this property is set to <see langword="true" />, the object is considered active and should perform
        /// any necessary initialization or load operations. When set to <see langword="false" />, the object should
        /// perform cleanup operations.
        /// </remarks>
        bool IsActive { get; set; }

        /// <summary>
        /// Notifies that the value for <see cref="IsActive"/> property has changed.
        /// </summary>
        /// <remarks>
        /// This event is raised whenever the <see cref="IsActive"/> property value changes, allowing listeners
        /// to respond to activation and deactivation events.
        /// </remarks>
        event EventHandler IsActiveChanged;
    }
}
