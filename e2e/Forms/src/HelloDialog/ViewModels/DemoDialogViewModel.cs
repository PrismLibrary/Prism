using System;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;

namespace HelloDialog.ViewModels
{
    public class DemoDialogViewModel : BindableBase, IDialogAware
    {
        public DemoDialogViewModel()
        {
            CloseCommand = new DelegateCommand(() => RequestClose.Invoke());
        }

        private string title = "Message";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public DelegateCommand CloseCommand { get; }

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            Console.WriteLine("The Demo Dialog has been closed...");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}
