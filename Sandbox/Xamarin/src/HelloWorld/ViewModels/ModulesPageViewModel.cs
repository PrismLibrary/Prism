using System.Collections.Generic;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;

namespace HelloWorld.ViewModels
{
    public class ModulesPageViewModel : BindableBase, IDestructible
    {
        private IModuleManager _moduleManager { get; }
        private IModuleCatalog _moduleCatalog { get; }

        public ModulesPageViewModel(IModuleManager moduleManager, IModuleCatalog moduleCatalog)
        {
            _moduleManager = moduleManager;
            _moduleCatalog = moduleCatalog;

            _moduleManager.LoadModuleCompleted += OnModuleLoaded;
            Modules = moduleCatalog.Modules;

            LoadModule = new DelegateCommand<IModuleInfo>(LoadModuleExecuted, i => i.State == ModuleState.NotStarted);
        }

        private IEnumerable<IModuleInfo> _modules;
        public IEnumerable<IModuleInfo> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        public DelegateCommand<IModuleInfo> LoadModule { get; }

        private void LoadModuleExecuted(IModuleInfo moduleInfo)
        {
            _moduleManager.LoadModule(moduleInfo.ModuleName);
        }

        private void OnModuleLoaded(object sender, LoadModuleCompletedEventArgs e)
        {
            Modules = _moduleCatalog.Modules;
        }

        public void Destroy()
        {
            _moduleManager.LoadModuleCompleted -= OnModuleLoaded;
        }
    }
}
