using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Specifies the <see cref="Prism.Navigation.IPageNavigationProvider"/> to use while navigating to the decorated <see cref="Xamarin.Forms.Page"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PageNavigationProviderAttribute : Attribute
    {
        /// <summary>
        /// The type that implements the <see cref="Prism.Navigation.IPageNavigationProvider"/> interface
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Create a new instance of the <see cref="PageNavigationProviderAttribute"/>
        /// </summary>
        /// <param name="pageProviderType">The type of the <see cref="Prism.Navigation.IPageNavigationProvider"/> to use.</param>
        public PageNavigationProviderAttribute(Type pageProviderType)
        {
            if (pageProviderType == null)
                throw new ArgumentNullException("pageProviderType");

            Type = pageProviderType;
        }
    }
}
