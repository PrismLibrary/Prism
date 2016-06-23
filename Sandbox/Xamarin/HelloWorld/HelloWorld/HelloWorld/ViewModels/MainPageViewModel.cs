using HelloWorld.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Modularity;
using System;

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
    }
}
