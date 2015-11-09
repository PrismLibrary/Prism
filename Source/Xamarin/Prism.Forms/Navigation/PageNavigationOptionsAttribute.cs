using System;

namespace Prism.Navigation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PageNavigationOptionsAttribute : Attribute
    {
        public bool Animated { get; set; } = true;

        public bool UseModalNavigation { get; private set; } = true;

        public Type PageNavigationProviderType { get; set; }

        public PageNavigationOptionsAttribute(bool useModalNavigation)
        {
            UseModalNavigation = useModalNavigation;
        }
    }
}
