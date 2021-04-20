namespace Prism.Xaml
{
    /// <summary>
    /// Target BindingContext behavior for the <see cref="ParentPageAwareExtension{T}" />
    /// </summary>
    public enum TargetBindingContext
    {
        /// <summary>
        /// Use the Target Element's Binding Context
        /// </summary>
        Element = 0,

        /// <summary>
        /// Use the Parent Page's Binding Context (usually the ViewModel)
        /// </summary>
        Page = 1
    }
}
