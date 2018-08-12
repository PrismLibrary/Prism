using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism;
using Prism.AppModel;
using System.Diagnostics;

namespace ModuleA.ViewModels
{
    public class ViewAViewModel : BindableBase, INavigationAware, IActiveAware, IApplicationLifecycleAware, IPageLifecycleAware
    {
        private readonly INavigationService _navigationService;

        string _title = "View A";
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

        public DelegateCommand NavigateCommand { get; set; }

        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand ResetCommand { get; private set; }

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

        public ViewAViewModel(INavigationService navigationService, IApplicationCommands applicationCommands)
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
            Title = "View A";
        }

        private void Save()
        {
            Title = "Saved";
        }

        async void Navigate()
        {
            CanNavigate = false;
            await _navigationService.NavigateAsync($"ViewB/ViewC");
            CanNavigate = true;
        }

        void OnActiveChanged()
        {
            SaveCommand.IsActive = IsActive;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var navigationMode = parameters.GetNavigationMode();
            if (navigationMode == NavigationMode.Back)
                Title = "Went Back";
            else if (navigationMode == NavigationMode.New)
                Title = "Went to New Page";
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public void OnResume()
        {
            Title = "Application resumed";
        }

        public void OnSleep()
        {
            Title = "Aplpication went to sleep";
        }

        public void OnAppearing()
        {
            Debug.WriteLine("ViewA is appearing");
        }

        public void OnDisappearing()
        {
            Debug.WriteLine("ViewA is disappearing");
        }
    }
}
