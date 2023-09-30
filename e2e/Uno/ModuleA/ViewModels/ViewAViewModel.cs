using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleA.ViewModels;

internal class ViewAViewModel : ViewModelBase
{
    public ViewAViewModel(IRegionManager regionManager)
        : base(regionManager)
    {
    }
}
