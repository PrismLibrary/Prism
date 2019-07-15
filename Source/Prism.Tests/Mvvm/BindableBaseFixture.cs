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

            Assert.Equal(0, mockViewModel.MockProperty);

            mockViewModel.MockProperty = value;
            Assert.Equal(value, mockViewModel.MockProperty);
        }

        [Fact]
        public void SetNestedPropertyMethodShouldSetTheNewValue()
        {
            int value = 10;
            MockViewModel mockViewModel = new MockViewModel
            {
                MockNestedModel = new MockNestedModel()
            };

            Assert.Equal(0, mockViewModel.MockNestedProperty);

            mockViewModel.MockNestedProperty = value;
            Assert.Equal(value, mockViewModel.MockNestedProperty);
        }

        [Fact]
        public void SetPropertyMethodShouldNotSetTheNewValue()
        {
            int value = 10, newValue = 10;
            MockViewModel mockViewModel = new MockViewModel();
            mockViewModel.MockProperty = value;

            bool invoked = false;
            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.MockProperty = newValue;

            Assert.False(invoked);
            Assert.Equal(value, mockViewModel.MockProperty);
        }

        [Fact]
        public void SetNestedPropertyMethodShouldNotSetTheNewValue()
        {
            int value = 10, newValue = 10;
            MockViewModel mockViewModel = new MockViewModel
            {
                MockNestedModel = new MockNestedModel()
            };
            mockViewModel.MockNestedProperty = value;

            bool invoked = false;
            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockNestedProperty")) invoked = true; };
            mockViewModel.MockNestedProperty = newValue;

            Assert.False(invoked);
            Assert.Equal(value, mockViewModel.MockNestedProperty);
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
        public void SetNestedPropertyMethodShouldRaisePropertyRaised()
        {
            bool invoked = false;
            MockViewModel mockViewModel = new MockViewModel
            {
                MockNestedModel = new MockNestedModel()
            };

            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockNestedProperty")) invoked = true; };
            mockViewModel.MockNestedProperty = 10;

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

        [Fact]
        public void OnNestedPropertyChangedShouldExtractPropertyNameCorrectly()
        {
            bool invoked = false;
            MockViewModel mockViewModel = new MockViewModel
            {
                MockNestedModel = new MockNestedModel()
            };

            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockNestedProperty")) invoked = true; };
            mockViewModel.InvokeOnNestedPropertyChanged();

            Assert.True(invoked);
        }
    }
}
