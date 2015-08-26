using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.UI.ApplicationSettings;
using HelloWorld.Services;
using Prism.Windows.Mvvm;
using Prism.Windows.Interfaces;
using Prism.Commands;

namespace HelloWorld.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        private readonly IDataRepository _dataRepository;
        
        public MainPageViewModel(IDataRepository dataRepository, INavigationService navService)
        {
            _dataRepository = dataRepository;

            NavigateCommand = new DelegateCommand(() => navService.Navigate("UserInput", null));
        }

        public DelegateCommand NavigateCommand { get; set; }

        public List<string> DisplayItems
        {
            get { return _dataRepository.GetFeatures(); }
        }
    }
}
