using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Input;

namespace Prism.ShowDialog
{
    public class Notification : BindableBase, INotification, IShowDialogRequestAware
    {
        public Notification()
        {
            this.AcceptCommand = new DelegateCommand(() => this.OnAccept(), () => this.CanAccept());
        }

        public object Content { get; set; }

        public DelegateCommand AcceptCommand { get; private set; }

        public Action FinishInteraction { get; set; }

        protected virtual void OnAccept()
        {
            this.FinishInteraction?.Invoke();
        }

        protected virtual bool CanAccept() => true;
    }
}
