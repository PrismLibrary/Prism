using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Prism.Modularity
{
    // IList must be supported in Silverlight 2 to be able to add items from XAML
    public interface IModuleInfoGroup : IModuleCatalogItem, IList<IModuleInfo>, IList
    {
        InitializationMode InitializationMode { get; set; }

        string Ref { get; set; }
    }
}
