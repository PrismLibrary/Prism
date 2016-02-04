using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Issue322
{
    public class ViewModel : INotifyPropertyChanged
    {
        public DelegateCommand _RegisterCommand;
        private string _FirstName;
        private string _LastName;
        private string _Result;
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            _RegisterCommand = new DelegateCommand(ExecuteRegisterCommand, CanExecuteRegisterCommand);
            _RegisterCommand.ObservesProperty(() => this.FirstName);
            _RegisterCommand.ObservesProperty(() => this.LastName);
            new Task(MyThread).Start();
        }

        public void MyThread()
        {
            Thread.Sleep(5000);
            this.FirstName = "If you see this First Name value - the test has passed.";
            this.LastName = "If you see this Last Name value - the test has passed.";
        }

        public DelegateCommand RegisterCommand
        {
            get { return this._RegisterCommand; }
        }

        public string FirstName
        {
            get { return this._FirstName; }
            set { this._FirstName = value; OnPropertyChanged("FirstName"); }
        }

        public string LastName
        {
            get { return this._LastName; }
            set { this._LastName = value; OnPropertyChanged("LastName"); }
        }

        public string Result
        {
            get { return this._Result; }
            set { this._Result = value; OnPropertyChanged("Result"); }
        }

        public bool CanExecuteRegisterCommand()
        {
            return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName);
        }

        public void ExecuteRegisterCommand()
        {
            Result = "Registered";
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
