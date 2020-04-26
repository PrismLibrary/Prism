using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using HelloWorld.Core;

namespace HelloWorld.Modules.ModuleA.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private DelegateCommand _showDialogCommand;
        private readonly IDialogService _dialogService;

        public DelegateCommand ShowDialogCommand =>
            _showDialogCommand ?? (_showDialogCommand = new DelegateCommand(ExecuteShowDialogCommand));

        void ExecuteShowDialogCommand()
        {
            _dialogService.ShowNotification("Hello There!", r =>
            {
                if (r.Result == ButtonResult.OK)
                    Message = "OK was clicked";
                else
                    Message = "Something else was clicked";
            });
        }

        public ViewAViewModel(IDialogService dialogService)
        {
            Message = "Hello from ViewA in Module A";
            _dialogService = dialogService;
        }
    }
}
