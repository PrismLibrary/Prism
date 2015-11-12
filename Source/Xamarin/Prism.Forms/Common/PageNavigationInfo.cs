using Prism.Navigation;
using System;

namespace Prism.Common
{
    public class PageNavigationInfo
    {
        public PageNavigationOptionsAttribute NavigationOptions { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}
