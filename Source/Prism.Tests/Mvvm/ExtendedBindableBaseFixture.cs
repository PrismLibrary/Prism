using Xunit;
using System.Collections.Generic;
using System.Linq;
using Prism.Tests.Mocks.ViewModels;

namespace Prism.Tests.Mvvm
{
    public class ExtendedBindableBaseFixture
    {
        [Fact]
        public void SetPropertyMethodShouldSetTheNewValue()
        {
            int value = 10;
            MockExtendedViewModel mockViewModel = new MockExtendedViewModel();

            Assert.Equal(0, mockViewModel.FirstProperty);

            mockViewModel.FirstProperty = value;
            Assert.Equal(value, mockViewModel.FirstProperty);
        }

        [Fact]
        public void SetPropertyMethodShouldRiseNotifyProperty()
        {
            List<string> properties = new List<string>();
            MockExtendedViewModel mockViewModel = new MockExtendedViewModel();
            mockViewModel.PropertyChanged += (se, ev) => { properties.Add(ev.PropertyName); };
            mockViewModel.FirstProperty = 10;
            Assert.Contains(properties, a => a == nameof(MockExtendedViewModel.FirstProperty));
            Assert.Contains(properties, a => a == nameof(MockExtendedViewModel.ThirdProperty));
        }

        [Fact]
        public void SetPropertyMethodShouldRiseDependsOnProperty()
        {
            List<string> properties = new List<string>();
            MockExtendedViewModel mockViewModel = new MockExtendedViewModel();
            mockViewModel.PropertyChanged += (se, ev) => { properties.Add(ev.PropertyName); };
            mockViewModel.FirstProperty = 10;
            Assert.Contains(properties, a => a == nameof(MockExtendedViewModel.FirstProperty));
            Assert.Contains(properties, a => a == nameof(MockExtendedViewModel.SecondProperty));
        }

        [Fact]
        public void SetPropertyMethodWiduthAttrubte()
        {
            List<string> properties = new List<string>();
            MockExtendedViewModel mockViewModel = new MockExtendedViewModel();
            mockViewModel.PropertyChanged += (se, ev) => { properties.Add(ev.PropertyName); };
            mockViewModel.ThirdProperty = 10;
            Assert.Contains(properties, a => a == nameof(MockExtendedViewModel.ThirdProperty));
            Assert.Single(properties);
        }

    }
}
