using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.ShowDialog
{
    public class ShowDialogRequestEventArgs : EventArgs
    {
        public ShowDialogRequestEventArgs(string title, object viewModel, Action callback)
        {
            this.Title = title;
            this.ViewModel = viewModel;
            this.Callback = callback;
        }

        public string Title { get; private set; }
        public object ViewModel { get; private set; }
        public Action Callback { get; private set; }
    }
}
