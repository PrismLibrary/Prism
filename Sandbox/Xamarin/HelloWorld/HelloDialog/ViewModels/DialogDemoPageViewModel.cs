using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloDialog.ViewModels
{
    public class DialogDemoPageViewModel : BindableBase
    {
        private IPageDialogService _pageDialogService { get; }
        public DialogDemoPageViewModel(IDialogService dialogService, IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;

            ShowUserAlertCommand = new DelegateCommand(() => dialogService.ShowDialog("UserAlert", OnAlertClosed));
        }

        public string TestMessage => "This is a sample message from a binding in the ViewModel";

        public DelegateCommand ShowUserAlertCommand { get; }

        private async void OnAlertClosed(IDialogResult result)
        {
            await _pageDialogService.DisplayAlertAsync("Success", $"You entered {result.Parameters.GetValue<string>("name")}", "Ok");
        }
    }
}
