using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConfigModuleEditor.Models
{
    public class ModuleDependencyInfo : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
