using Xunit;
using Prism.Tests.Mocks.ViewModels;

namespace Prism.Tests.Mvvm
{
    public class ChangingBindableBaseFixture
    {
        [Fact]
        public void SetPropertyMethodShouldSetTheNewValue()
        {
            int value = 10;
            MockChangingViewModel mockViewModel = new MockChangingViewModel();

            Assert.Equal(0, mockViewModel.MockProperty);

            mockViewModel.MockProperty = value;
            Assert.Equal(value, mockViewModel.MockProperty);
        }

        [Fact]
        public void SetPropertyMethodShouldNotSetTheNewValue()
        {
            bool invoked = false;
            int value = 10, newValue = 10;
            MockChangingViewModel mockViewModel = new MockChangingViewModel();
            mockViewModel.MockProperty = value;

            mockViewModel.PropertyChanging += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.MockProperty = newValue;

            Assert.False(invoked);
            Assert.Equal(value, mockViewModel.MockProperty);
        }

        [Fact]
        public void SetPropertyMethodShouldRaisePropertyChanging()
        {
            bool invoked = false;
            MockChangingViewModel mockViewModel = new MockChangingViewModel();

            mockViewModel.PropertyChanging += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.MockProperty = 10;

            Assert.True(invoked);
        }

        [Fact]
        public void OnPropertyChangedShouldExtractPropertyNameCorrectly()
        {
            bool invoked = false;
            MockChangingViewModel mockViewModel = new MockChangingViewModel();

            mockViewModel.PropertyChanging += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.InvokeOnPropertyChanging();

            Assert.True(invoked);
        }
    }
}
