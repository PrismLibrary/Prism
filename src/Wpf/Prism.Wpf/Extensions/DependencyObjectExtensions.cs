using System;
using System.Collections.Generic;
using System.Text;

#if HAS_WINUI
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

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
        public static bool HasBinding(this FrameworkElement instance, DependencyProperty property)
#if HAS_WINUI
            => instance.GetBindingExpression(property) != null;
#else
            => BindingOperations.GetBinding(instance, property) != null;
#endif
    }
}
