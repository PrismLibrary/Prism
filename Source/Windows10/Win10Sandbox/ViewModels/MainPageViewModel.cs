using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Runtime.CompilerServices;

namespace Win10Sandbox.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public DelegateCommand LoginCommand { get; set; }

        public MainPageViewModel()
        {
            LoginCommand = new DelegateCommand(Login, CanLogin);
        }

        private bool CanLogin()
        {
            return !String.IsNullOrWhiteSpace(Username) && !String.IsNullOrWhiteSpace(Password);
        }

        private void Login()
        {
            //do something
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (base.SetProperty<T>(ref storage, value, propertyName))
            {
                LoginCommand.RaiseCanExecuteChanged();
                return true;
            }
            else
                return false;
        }
    }
}
