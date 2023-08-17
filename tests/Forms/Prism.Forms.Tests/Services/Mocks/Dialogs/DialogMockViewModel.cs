using Prism.Dialogs;
using Prism.Mvvm;

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

        public DialogCloseListener RequestClose { get; set; }

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
