using System;
using Xamarin.Forms;

namespace Prism.Regions
{
    /// <summary>
    /// Defines a class that wraps an item and adds metadata for it.
    /// </summary>
    public class ItemMetadata : BindableObject
    {
        /// <summary>
        /// The name of the wrapped item.
        /// </summary>
        public static readonly BindableProperty NameProperty =
            BindableProperty.Create(nameof(Name), typeof(string), typeof(ItemMetadata), null);

        /// <summary>
        /// Value indicating whether the wrapped item is considered active.
        /// </summary>
        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(ItemMetadata), propertyChanged: BindablePropertyChanged);

        /// <summary>
        /// Initializes a new instance of <see cref="ItemMetadata"/>.
        /// </summary>
        /// <param name="item">The item to wrap.</param>
        public ItemMetadata(VisualElement item)
        {
            // check for null
            Item = item;
        }

        /// <summary>
        /// Gets the wrapped item.
        /// </summary>
        /// <value>The wrapped item.</value>
        public VisualElement Item { get; private set; }

        /// <summary>
        /// Gets or sets a name for the wrapped item.
        /// </summary>
        /// <value>The name of the wrapped item.</value>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the wrapped item is considered active.
        /// </summary>
        /// <value><see langword="true" /> if the item should be considered active; otherwise <see langword="false" />.</value>
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        /// <summary>
        /// Occurs when metadata on the item changes.
        /// </summary>
        public event EventHandler MetadataChanged;

        /// <summary>
        /// Explicitly invokes <see cref="MetadataChanged"/> to notify listeners.
        /// </summary>
        public void InvokeMetadataChanged()
        {
            MetadataChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void BindablePropertyChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject is ItemMetadata itemMetadata)
            {
                itemMetadata.InvokeMetadataChanged();
            }
        }
    }
}
