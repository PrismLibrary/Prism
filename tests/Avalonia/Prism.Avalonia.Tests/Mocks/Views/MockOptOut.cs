using Avalonia.Controls;
using Prism.Mvvm;

namespace Prism.Avalonia.Tests.Mocks.Views
{
    public class MockOptOut : Control
    {
        public MockOptOut()
        {
            ViewModelLocator.SetAutoWireViewModel(this, false);
        }
    }
}
