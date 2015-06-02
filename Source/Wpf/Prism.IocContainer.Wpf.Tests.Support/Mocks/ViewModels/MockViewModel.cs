using Prism.Mvvm;

namespace Prism.IocContainer.Wpf.Tests.Support.Mocks.ViewModels
{
    public class MockViewModel : BindableBase
    {
        private IService _mockService;
        public IService MockService
        {
            get { return _mockService; }
            set { SetProperty(ref _mockService, value); }
        }

        public MockViewModel(IService mockService)
        {
            _mockService = mockService;
        }
    }
}
