using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.ShowDialog
{
    interface IConfirmation : INotification
    {
        bool Confirmed { get; set; }
    }
}
