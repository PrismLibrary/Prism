namespace Prism.AppModel;

/// <summary>
/// Specifies the layout direction requested for a view.
/// </summary>
/// <remarks>These values correspond to <see cref="Microsoft.Maui.FlowDirection"/>.
/// Set the view's flow direction explicitly when right-to-left layout is required.</remarks>
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
