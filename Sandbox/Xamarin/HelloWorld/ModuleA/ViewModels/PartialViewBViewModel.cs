using System;
using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace ModuleA.ViewModels
{
    public class PartialViewBViewModel : BindableBase
    {
        private INavigationService _navigationService { get; }

        public PartialViewBViewModel(INavigationService navigationService, IApplicationCommands applicationCommands)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand(Navigate).ObservesCanExecute(() => CanNavigate);
            SaveCommand = new DelegateCommand(Save);
            ResetCommand = new DelegateCommand(Reset);

            applicationCommands.SaveCommand.RegisterCommand(SaveCommand);
            applicationCommands.ResetCommand.RegisterCommand(ResetCommand);
        }

        private string _title = "View B";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _canNavigate = true;
        public bool CanNavigate
        {
            get { return _canNavigate; }
            set { SetProperty(ref _canNavigate, value); }
        }

        public event EventHandler IsActiveChanged;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
                OnActiveChanged();
            }
        }

        public DelegateCommand NavigateCommand { get; }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand ResetCommand { get; }

        private void Reset()
        {
            Title = "View B";
        }

        async void Navigate()
        {
            CanNavigate = false;
            await _navigationService.NavigateAsync("ViewA");
            CanNavigate = true;
        }

        private void Save()
        {
            Title = "Saved";
        }

        void OnActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
            SaveCommand.IsActive = IsActive;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            Debug.WriteLine("Navigated to ViewB");
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public void OnAppearing()
        {
            Debug.WriteLine("ViewB is appearing");
        }

        public void OnDisappearing()
        {
            Debug.WriteLine("ViewB is disappearing");
        }
    }
}
