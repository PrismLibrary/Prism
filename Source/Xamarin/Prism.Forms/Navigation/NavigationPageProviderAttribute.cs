using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Specifies the <see cref="Prism.Navigation.INavigationPageProvider"/> to use while navigating to the decorated <see cref="Xamarin.Forms.Page"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NavigationPageProviderAttribute : Attribute
    {
        /// <summary>
        /// The type that implements the <see cref="Prism.Navigation.INavigationPageProvider"/> interface
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Create a new instance of the <see cref="NavigationPageProviderAttribute"/>
        /// </summary>
        /// <param name="pageProviderType">The type of the <see cref="Prism.Navigation.INavigationPageProvider"/> to use.</param>
        public NavigationPageProviderAttribute(Type pageProviderType)
        {
            if (pageProviderType == null)
                throw new ArgumentNullException("pageProviderType");

            Type = pageProviderType;
        }
    }
}
