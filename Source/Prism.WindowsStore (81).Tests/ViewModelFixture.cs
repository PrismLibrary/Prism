// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using System.Collections.Generic;
using System.Linq;
using Prism.WindowsStore.Tests.Mocks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.UI.Xaml.Navigation;

namespace Prism.WindowsStore.Tests
{
    [TestClass]
    public class ViewModelFixture
    {
        [TestMethod]
        public void OnNavigatedFrom_With_No_RestorableStateAttributes()
        {
            var vm = new MockViewModelWithNoRestorableStateAttributes()
            {
                Title = "MyMock",
                Description = "MyDescription",
            };

            var result = new Dictionary<string, object>();
            
            vm.OnNavigatedFrom(result, true);

            Assert.IsTrue(result.Keys.Count == 0);
        }

        [TestMethod]
        public void OnNavigatedFrom_With_RestorableStateAttributes()
        {
            var vm = new MockViewModelWithRestorableStateAttributes()
            {
                Title = "MyMock",
                Description = "MyDescription",
            };
            var result = new Dictionary<string, object>();

            vm.OnNavigatedFrom(result, true);

            Assert.IsTrue(result.Keys.Count == 2);
            Assert.AreEqual("MyMock", result["Title"]);
            Assert.AreEqual("MyDescription", result["Description"]);
        }

        [TestMethod]
        public void OnNavigatedTo_With_No_RestorableStateAttributes()
        {
            var viewModelState = new Dictionary<string, object>();
            viewModelState.Add("Title", "MyMock");
            viewModelState.Add("Description", "MyDescription");

            var viewState = new Dictionary<string, object>();
            viewState.Add("Tests.Mocks.MockViewModelWithNoResumableStateAttributes1", viewModelState);

            var vm = new MockViewModelWithNoRestorableStateAttributes();
            vm.OnNavigatedTo(null, NavigationMode.Back, viewState);

            Assert.IsNull(vm.Title);
            Assert.IsNull(vm.Description);
        }


        [TestMethod]
        public void OnNavigatedTo_With_RestorableStateAttribute()
        {
            var viewModelState = new Dictionary<string, object>();
            viewModelState.Add("Title", "MyMock");
            viewModelState.Add("Description", "MyDescription");

            var vm = new MockViewModelWithRestorableStateAttributes();
            vm.OnNavigatedTo(null, NavigationMode.Back, viewModelState);

            Assert.AreEqual(vm.Title, viewModelState["Title"]);
            Assert.AreEqual(vm.Description, viewModelState["Description"]);
        }

        [TestMethod]
        public void OnNavigatedTo_With_RestorableStateCollection()
        {
            var childViewModelState = new Dictionary<string, object>();
            childViewModelState.Add("Title", "MyChildMock");
            childViewModelState.Add("Description", "MyChildDescription");

            var viewModelState = new Dictionary<string, object>();
            viewModelState.Add("Tests.Mocks.MockViewModelWithResumableStateCollection1", childViewModelState);

            var viewState = new Dictionary<string, object>();
            viewState.Add("MyEntityId", viewModelState);

            var vm = new MockViewModelWithRestorableStateCollection()
            {
                ChildViewModels = new List<BindableBase>()
                {
                    new MockViewModelWithRestorableStateAttributes
                    {
                        Title = "MyChildMock",
                        Description = "MyChildDescription"
                    }
                }
            };
            vm.OnNavigatedTo(null, NavigationMode.Back, viewState);

            var childViewModel = (MockViewModelWithRestorableStateAttributes)vm.ChildViewModels.FirstOrDefault();

            Assert.AreEqual(childViewModel.Title, childViewModelState["Title"]);
            Assert.AreEqual(childViewModel.Description, childViewModelState["Description"]);
        }
    }
}
