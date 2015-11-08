using System;

namespace Prism.Navigation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PageNavigationParametersAttribute : Attribute
    {
        public bool UseModalNavigation { get; set; } = true;

        public bool Animated { get; set; } = true;

        public PageNavigationParametersAttribute(bool useModalNavigation)
        {
            UseModalNavigation = useModalNavigation;
        }

        public PageNavigationParametersAttribute(bool useModalNavigation, bool animated) : this (useModalNavigation)
        {
            Animated = animated;
        }
    }
}
