using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.ShowDialog
{
    public interface IYesNoCancel : INotification
    {
        bool? Confirmed { get; set; }
    }
}
