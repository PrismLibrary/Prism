using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Navigation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NavigationServiceParametersAttribute : Attribute
    {
        public bool UseModalNavigation { get; set; } = true;

        public NavigationServiceParametersAttribute(bool useModalNavigation)
        {
            UseModalNavigation = useModalNavigation;
        }
    }
}
