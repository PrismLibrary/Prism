using Avalonia;
using Avalonia.Controls;

namespace Prism
{
    /// <summary>AvaloniaObject Extensions.</summary>
    /// <remarks>Equivalent to WPF's DependencyObject</remarks>
    internal static partial class AvaloniaObjectExtensions
    {
        /// <summary>Determines if a <see cref="AvaloniaProperty"/> has a binding set.</summary>
        /// <param name="instance">The to use to search for the property.</param>
        /// <param name="property">The property to search.</param>
        /// <returns><c>true</c> if there is an active binding, otherwise <c>false</c>.</returns>
        public static bool HasBinding(this Control instance, AvaloniaProperty property)
            => instance.GetBindingObservable(property) != null;
    }
}
