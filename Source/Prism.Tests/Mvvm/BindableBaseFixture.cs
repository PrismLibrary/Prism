// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Tests.Mocks.ViewModels;

namespace Prism.Tests.Mvvm
{
    [TestClass]
    public class BindableBaseFixture
    {
        [TestMethod]
        public void SetPropertyMethodShouldSetTheNewValue()
        {
            int value = 10;
            MockViewModel mockViewModel = new MockViewModel();

            Assert.AreEqual(mockViewModel.MockProperty, 0);

            mockViewModel.MockProperty = value;
            Assert.AreEqual(mockViewModel.MockProperty, value);
        }

        [TestMethod]
        public void SetPropertyMethodShouldRaisePropertyRaised()
        {
            bool invoked = false;
            MockViewModel mockViewModel = new MockViewModel();

            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.MockProperty = 10;

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void OnPropertyChangedShouldExtractPropertyNameCorrectly()
        {
            bool invoked = false;
            MockViewModel mockViewModel = new MockViewModel();

            mockViewModel.PropertyChanged += (o, e) => { if (e.PropertyName.Equals("MockProperty")) invoked = true; };
            mockViewModel.InvokeOnPropertyChanged();

            Assert.IsTrue(invoked);
        }
    }
}
