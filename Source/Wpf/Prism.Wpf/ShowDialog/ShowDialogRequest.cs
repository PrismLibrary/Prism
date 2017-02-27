using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.ShowDialog
{
    public class ShowDialogRequest<T> : IShowDialogRequest
    {
        public event EventHandler<ShowDialogRequestEventArgs> Raised;

        public void Raise(string title, T viewModel, Action<T> callback)
        {
            this.Raised?.Invoke(this, new ShowDialogRequestEventArgs(title, viewModel, () => { if (callback != null) callback(viewModel); } ));
        }
    }
}
