using System.Windows;
using Prism.Mvvm;

namespace Prism.Wpf.Tests.Mocks.Views
{
    public class MockOptOut : FrameworkElement
    {
        public MockOptOut()
        {
            ViewModelLocator.SetAutoWireViewModel(this, false);
        }
    }
}
