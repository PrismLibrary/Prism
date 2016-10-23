using HelloWorld.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Modularity;
using System;
using Prism.Navigation;
using System.Threading.Tasks;

namespace HelloWorld.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IModuleManager _moduleManager;

        string _title = "Main Page";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand LoadModuleACommand { get; set; }

        public MainPageViewModel(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
            LoadModuleACommand = new DelegateCommand(LoadModuleA);
        }

        void LoadModuleA()
        {
            //_moduleManager.LoadModule("ModuleA");
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            Title = "OnNavigating To";
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            Title = "OnNavigated To";
        }
    }
}
