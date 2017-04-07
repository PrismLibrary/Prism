using Prism.Mvvm;

namespace ModuleA.ViewModels
{
    public class MyTabbedPageViewModel : BindableBase
    {
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

        public MyTabbedPageViewModel(IApplicationCommands applicationCommands)
        {
            _applicationCommands = applicationCommands;
        }
    }
}
