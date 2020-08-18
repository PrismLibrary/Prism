using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace HelloRegions.ViewModels
{
    public interface IRefreshable
    {
        ICommand RefreshCommand { get; }
        bool IsRefreshing { get; }
    }
}
