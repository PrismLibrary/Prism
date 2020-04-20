using Prism.Mvvm;
using Prism.Regions.Navigation;

namespace HelloRegions.ViewModels
{
    public class ViewModelBase : BindableBase, IRegionAware
    {
        public string Title { get; set; }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public bool IsNavigationTarget(INavigationContext navigationContext) =>
            false;

        public void OnNavigatedFrom(INavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(INavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("message"))
                Message = navigationContext.Parameters.GetValue<string>("message");
            else
                Message = "No Message provided...";
        }
    }
}
