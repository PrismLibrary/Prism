using Prism.Mvvm;
using System;

namespace Prism.Services.Dialogs
{
    public class DialogViewModelBase : BindableBase, IDialogAware
    {
        private string _iconSource;
        public string IconSource
        {
            get { return _iconSource; }
            set { SetProperty(ref _iconSource, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}
