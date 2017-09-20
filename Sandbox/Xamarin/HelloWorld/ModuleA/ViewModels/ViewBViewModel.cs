using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Diagnostics;
using System;
using Prism;

namespace ModuleA.ViewModels
{
    public class ViewBViewModel : BindableBase, INavigationAware, IActiveAware
    {
        private readonly INavigationService _navigationService;

        string _title = "View B";
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

        bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
                OnActiveChanged();
            }
        }

        public DelegateCommand NavigateCommand { get; set; }

        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

        public ViewBViewModel(INavigationService navigationService, IApplicationCommands applicationCommands)
        {
            _navigationService = navigationService;
            NavigateCommand = new DelegateCommand(Navigate).ObservesCanExecute(() => CanNavigate);
            SaveCommand = new DelegateCommand(Save);
            ResetCommand = new DelegateCommand(Reset);

            applicationCommands.SaveCommand.RegisterCommand(SaveCommand);
            applicationCommands.ResetCommand.RegisterCommand(ResetCommand);
        }

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
            SaveCommand.IsActive = IsActive;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            Debug.WriteLine("Navigated to ViewB");
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
