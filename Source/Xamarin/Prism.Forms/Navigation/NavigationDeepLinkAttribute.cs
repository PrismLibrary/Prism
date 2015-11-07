using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NavigationDeepLinkAttribute : Attribute
    {
        public NavigationMode NavigationMode { get; set; }

        public NavigationDeepLinkAttribute(NavigationMode navigationMode)
        {
            NavigationMode = navigationMode;
        }
    }

    public enum NavigationMode
    {
        Modal,
        Push
    }
}
