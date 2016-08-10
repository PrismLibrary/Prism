using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace $safeprojectname$.ViewModels
{
    public class MenuItemViewModel : ViewModelBase
    {
        public string DisplayName { get; set; }

        public string FontIcon { get; set; }

        public ICommand Command { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
