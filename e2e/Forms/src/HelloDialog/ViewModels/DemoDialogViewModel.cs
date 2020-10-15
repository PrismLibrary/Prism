using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Services.Dialogs;
using Prism.Navigation;
using Prism.AppModel;

namespace HelloDialog.ViewModels
{
    public class DemoDialogViewModel : BindableBase, IDialogAware
    {
        public DemoDialogViewModel()
        {
            CloseCommand = new DelegateCommand(() => RequestClose(null));
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

        public event Action<IDialogParameters> RequestClose;

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
