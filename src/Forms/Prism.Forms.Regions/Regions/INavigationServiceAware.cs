using System;
using System.Collections.Generic;
using System.Text;
using Prism.Navigation;

namespace Prism.Regions
{
    internal interface INavigationServiceAware
    {
        INavigationService NavigationService { get; set; }
    }
}
