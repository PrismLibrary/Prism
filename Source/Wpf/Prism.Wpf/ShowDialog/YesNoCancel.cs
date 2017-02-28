using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;

namespace Prism.ShowDialog
{
    public class YesNoCancel : Notification, IYesNoCancel
    {
        public YesNoCancel()
        {
            this.CancelCommand = new DelegateCommand(() => this.OnCancel(), () => this.CanCancel());
            this.RejectCommand = new DelegateCommand(() => this.OnReject(), () => this.CanReject());
        }

        public bool? Confirmed { get; set; }

        public DelegateCommand CancelCommand { get; private set; }

        public DelegateCommand RejectCommand { get; private set; }

        protected virtual void OnCancel()
        {
            this.Confirmed = null;
            this.FinishInteraction?.Invoke();
        }

        protected virtual void OnReject()
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

        protected virtual bool CanReject() => true;
    }
}
