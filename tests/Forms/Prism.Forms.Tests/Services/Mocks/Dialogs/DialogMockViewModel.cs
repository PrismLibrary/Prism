using System;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Dialogs;

namespace Prism.Forms.Tests.Services.Mocks.Dialogs
{
    public class DialogMockViewModel : BindableBase, IDialogAware
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public DialogCloseEvent RequestClose { get; set; }

        public bool CanClose { get; set; } = true;

        bool IDialogAware.CanCloseDialog() => CanClose;

        void IDialogAware.OnDialogClosed()
        {

        }

        void IDialogAware.OnDialogOpened(IDialogParameters parameters)
        {

        }

        public void SendRequestClose(IDialogParameters parameters = null) => RequestClose.Invoke(parameters);
    }
}
