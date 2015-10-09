using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Specifies the INavigationPageProvider to use while navigating to the decorated Page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NavigationPageProviderAttribute : Attribute
    {
        /// <summary>
        /// The type that implements the INavigationPageProvider interface
        /// </summary>
        public Type Type { get; set; }

        public NavigationPageProviderAttribute(Type pageProviderType)
        {
            if (pageProviderType == null)
                throw new ArgumentNullException("pageProviderType");

            Type = pageProviderType;
        }
    }
}
