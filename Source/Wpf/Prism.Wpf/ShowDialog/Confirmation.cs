using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;

namespace Prism.ShowDialog
{
    public class Confirmation : Notification, IConfirmation
    {
        public Confirmation()
        {
            this.CancelCommand = new DelegateCommand(() => this.OnCancel(), () => this.CanCancel());
        }

        public bool Confirmed { get; set; }

        public DelegateCommand CancelCommand { get; private set; }

        protected virtual void OnCancel()
        {
            this.Confirmed = false;
            this.FinishInteraction?.Invoke();
        }

        protected override void OnAccept()
        {
            this.Confirmed = true;
            base.OnAccept();
        }

        protected virtual bool CanCancel() => true;
    }
}
