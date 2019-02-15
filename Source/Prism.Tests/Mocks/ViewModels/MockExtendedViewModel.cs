

using Prism.Mvvm;
using System.ComponentModel;
using Prism.Attributes;

namespace Prism.Tests.Mocks.ViewModels
{
    public class MockExtendedViewModel : ExtendedBindableBase
    {
        
        [NotifyProperty(nameof(ThirdProperty))]
        public int FirstProperty
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        [DependsOnProperty(nameof(FirstProperty))]
        public int SecondProperty
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public int ThirdProperty
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
    }
}
