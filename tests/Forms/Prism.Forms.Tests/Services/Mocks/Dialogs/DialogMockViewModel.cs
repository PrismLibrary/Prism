using System;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Services.Dialogs;

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

        public event Action<IDialogParameters> RequestClose;

        public bool CanClose { get; set; } = true;

        bool IDialogAware.CanCloseDialog() => CanClose;

        void IDialogAware.OnDialogClosed()
        {

        }

        void IDialogAware.OnDialogOpened(IDialogParameters parameters)
        {

        }

        public void SendRequestClose(IDialogParameters parameters = null) => RequestClose(parameters);
    }
}
