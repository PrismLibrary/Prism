using Xunit;
using Prism.Tests.Mocks.ViewModels;

namespace Prism.Tests.Mvvm
{
    public class BindableBaseFixture
    {
        [Fact]
        public void SetPropertyMethodShouldSetTheNewValue()
        {
            int value = 10;
            MockViewModel mockViewModel = new MockViewModel();

            Assert.Equal(mockViewModel.MockProperty, 0);

            mockViewModel.MockProperty = value;
            Assert.Equal(mockViewModel.MockProperty, value);
        }

        [Fact]
        public void SetPropertyMethodShouldNotSetTheNewValue()
        {
            int value = 10;
            MockViewModel mockViewModel = new MockViewModel()
            {
                MockProperty = 10
            };
            bool invoked = false;
            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.MockProperty = value;

            Assert.False(invoked);
            Assert.Equal(mockViewModel.MockProperty, value);
        }

        [Fact]
        public void SetPropertyMethodShouldRaisePropertyRaised()
        {
            bool invoked = false;
            MockViewModel mockViewModel = new MockViewModel();

            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.MockProperty = 10;

            Assert.True(invoked);
        }

        [Fact]
        public void OnPropertyChangedShouldExtractPropertyNameCorrectly()
        {
            bool invoked = false;
            MockViewModel mockViewModel = new MockViewModel();

            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.InvokeOnPropertyChanged();

            Assert.True(invoked);
        }
    }
}
