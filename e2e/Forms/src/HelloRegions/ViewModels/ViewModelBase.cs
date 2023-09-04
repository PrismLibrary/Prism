using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;

namespace HelloRegions.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IRegionAware, Prism.IActiveAware
    {
        protected ViewModelBase()
        {
            GoBackCommand = new DelegateCommand(GoBack, () => Context?.NavigationService?.Journal?.CanGoBack ?? false)
                .ObservesProperty(() => Context);
            GoForwardCommand = new DelegateCommand(GoForward, () => Context?.NavigationService?.Journal?.CanGoForward ?? false)
                .ObservesProperty(() => Context);
        }

        public string Title { get; set; }

        private string _message;

        public event EventHandler IsActiveChanged;

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DelegateCommand GoBackCommand { get; }

        public DelegateCommand GoForwardCommand { get; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, OnIsActiveChanged);
        }

        private NavigationContext _context;
        private NavigationContext Context
        {
            get => _context;
            set => SetProperty(ref _context, value);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            Console.WriteLine($"{GetType().Name} IsNavigationTarget called");
            var lookingFor = navigationContext.NavigatedName();
            return _contextName == lookingFor;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Context = navigationContext;
            Console.WriteLine($"{GetType().Name} NavigatedFrom");
        }

        private string _contextName;
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Context = navigationContext;
            _contextName = navigationContext.NavigatedName();
            if (navigationContext.Parameters.ContainsKey("message"))
                Message = navigationContext.Parameters.GetValue<string>("message");
            else
                Message = "No Message provided...";

            Console.WriteLine($"{GetType().Name} NavigatedTo");
        }

        private void GoBack()
        {
            Context.NavigationService.Journal.GoBack();
        }

        private void GoForward()
        {
            Context.NavigationService.Journal.GoForward();
        }

        private void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
            Console.WriteLine($"{GetType().Name} is Active: {IsActive}");
        }
    }
}
