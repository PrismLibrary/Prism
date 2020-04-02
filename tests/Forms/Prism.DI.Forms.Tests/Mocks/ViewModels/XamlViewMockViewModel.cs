using Prism.Mvvm;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class XamlViewMockViewModel : BindableBase
    {
        private string _test = "Initial Value";
        public string Test
        {
            get => _test;
            set => SetProperty(ref _test, value);
        }
    }
}