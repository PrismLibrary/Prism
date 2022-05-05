using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace HelloWorld.ViewModels
{
    class ShellViewModel : BindableBase
    {
        public string Title { get; set; } = "Hello Uno for Prism";

        private DelegateCommand<string> _navigateCommand;
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand =>
            _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(ExecuteNavigateCommand));

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        void ExecuteNavigateCommand(string viewName)
        {
            _regionManager.RequestNavigate("ContentRegion", viewName);
        }
    }
}
