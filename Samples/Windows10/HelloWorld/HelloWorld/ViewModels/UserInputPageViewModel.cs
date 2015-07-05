using HelloWorld.Services;
using Prism.Commands;
using Prism.Windows.AppModel;
using Prism.Windows.Interfaces;
using Prism.Windows.Mvvm;

namespace HelloWorld.ViewModels
{
    public class UserInputPageViewModel : ViewModel
    {
        private readonly IDataRepository _dataRepository;
        private readonly INavigationService _navService;
        private string _VMState;

        public UserInputPageViewModel(IDataRepository dataRepository, INavigationService navService)
        {
            _navService = navService;
            _dataRepository = dataRepository;
            GoBackCommand = new DelegateCommand(_navService.GoBack);
        }

        public DelegateCommand GoBackCommand { get; set; }

        [RestorableState]
        public string VMState
        {
            get { return _VMState; }
            set { SetProperty(ref _VMState, value); }
        }

        public string ServiceState
        {
            get { return _dataRepository.GetUserEnteredData(); }
            set { _dataRepository.SetUserEnteredData(value); }
        }
    }
}
