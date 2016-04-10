using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConfigModuleEditor.Models
{
    public class ModuleInfo : INotifyPropertyChanged
    {
        private string _assemblyFile;
        public string AssemblyFile
        {
            get { return _assemblyFile; }
            set

            {
                _assemblyFile = value;
                OnPropertyChanged();
            }
        }

        private string _moduleType;
        public string ModuleType
        {
            get { return _moduleType; }
            set
            {
                _moduleType = value;
                OnPropertyChanged();
            }
        }

        private string _moduleName;
        public string ModuleName
        {
            get { return _moduleName; }
            set
            {
                _moduleName = value;
                OnPropertyChanged();
            }
        }

        private bool _startupLoaded = true;
        public bool StartupLoaded
        {
            get { return _startupLoaded; }
            set
            {
                _startupLoaded = value;
                OnPropertyChanged();
            }
        }

        private IList<ModuleDependencyInfo> _dependencies = new List<ModuleDependencyInfo>();
        public IList<ModuleDependencyInfo> Dependencies
        {
            get { return _dependencies; }
            set
            {
                _dependencies = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
