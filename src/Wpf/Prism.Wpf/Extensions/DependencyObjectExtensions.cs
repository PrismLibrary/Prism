namespace Prism
{
    internal static partial class DependencyObjectExtensions
    {
        /// <summary>
        /// Determines if a <see cref="DependencyProperty"/> has a binding set
        /// </summary>
        /// <param name="instance">The to use to search for the property</param>
        /// <param name="property">The property to search</param>
        /// <returns><c>true</c> if there is an active binding, otherwise <c>false</c></returns>
        public static bool HasBinding(this FrameworkElement instance, DependencyProperty property) =>
            BindingOperations.GetBinding(instance, property) is not null;
    }
}
