namespace Prism.AppModel
{
    /// <summary>
    /// Enumerates values that control the layout direction for views. This maps to
    /// the Xamarin.Forms.FlowDirection.
    /// </summary>
    /// <remarks>
    /// The following contains a few important points from Right-to-Left Localization.
    /// Developers should consult that document for more information about limitations
    /// of right-to-left support, and for requirements to implement right-to-left support
    /// on various target platforms.
    /// The default value of Xamarin.Forms.FlowDirection for a visual element that has
    /// no parent is Xamarin.Forms.FlowDirection.LeftToRight, even on platforms where
    /// Xamarin.Forms.Device.FlowDirection is Xamarin.Forms.FlowDirection.RightToLeft.
    /// Therefore, developers must deliberately opt in to right-to-left layout. Developers
    /// can choose right-to-left layout by setting the Xamarin.Forms.VisualElement.FlowDirection
    /// property of the root element to Xamarin.Forms.FlowDirection.RightToLeft to choose
    /// right-to-left layout, or to Xamarin.Forms.FlowDirection.MatchParent to match
    /// the device layout.
    /// </remarks>
    public enum FlowDirection
    {
        /// <summary>
        /// Indicates that the view's layout direction will match the parent view's layout
        /// direction.
        /// </summary>
        MatchParent = 0,

        /// <summary>
        /// Indicates that view will be laid out left to right. This is the default when
        /// the view has no parent.
        /// </summary>
        LeftToRight = 1,

        /// <summary>
        /// Indicates that view will be laid out right to left.
        /// </summary>
        RightToLeft = 2,
    }
}
