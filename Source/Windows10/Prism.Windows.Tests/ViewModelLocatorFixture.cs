using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Prism.Windows.Tests.Mocks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Prism.Mvvm;
using Prism.Windows.Mvvm;

namespace Prism.Windows.Tests
{
    [TestClass]
    public class ViewModelLocatorFixture
    {
        public IAsyncAction ExecuteOnUIThread(DispatchedHandler action)
        {
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        } 

        [TestMethod]
        public async Task AutoWireViewModel_With_Factory_Registration()
        {
            await ExecuteOnUIThread(() =>
            {
                var page = new MockPage();

                // Register the ViewModel to the page
                ViewModelLocationProvider.Register(typeof(MockPage).ToString(), () => new MockPageViewModel());

                // Fire AutoWireViewModelChanged
                ViewModelLocator.SetAutoWireViewModel(page, true);

                Assert.IsNotNull(page.DataContext);
                Assert.IsInstanceOfType(page.DataContext, typeof(MockPageViewModel));
            });
        }

        [TestMethod]
        public async Task AutoWireViewModel_With_Custom_Resolver()
        {
            await ExecuteOnUIThread(() =>
            {
                var page = new MockPage();

                // Set the ViewTypeToViewModelTypeResolver
                ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
                {
                    var viewName = viewType.FullName;

                    // The ViewModel is in the same namespace as the View
                    var viewModelName = String.Format("{0}ViewModel", viewName);
                    return Type.GetType(viewModelName);
                });

                // Fire AutoWireViewModelChanged
                ViewModelLocator.SetAutoWireViewModel(page, true);

                Assert.IsNotNull(page.DataContext);
                Assert.IsInstanceOfType(page.DataContext, typeof(MockPageViewModel));
            });
        }

        [TestMethod]
        public async Task AutoWireViewModel_With_Custom_Resolver_And_Factory()
        {
            await ExecuteOnUIThread(() =>
            {
                var page = new MockPage();

                // Set the ViewTypeToViewModelTypeResolver
                ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
                {
                    var viewName = viewType.FullName;

                    // The ViewModel is in the same namespace as the View
                    var viewModelName = String.Format("{0}ViewModel", viewName);
                    return Type.GetType(viewModelName);
                });

                // Set the ViewTypeToViewModelTypeResolver
                ViewModelLocationProvider.SetDefaultViewModelFactory((viewModelType) =>
                {
                    // The ViewModel has a constructor with no parameters
                    return Activator.CreateInstance(viewModelType) as ViewModel;
                });

                // Fire AutoWireViewModelChanged
                ViewModelLocator.SetAutoWireViewModel(page, true);

                Assert.IsNotNull(page.DataContext);
                Assert.IsInstanceOfType(page.DataContext, typeof(MockPageViewModel));
            });
        }
    }
}
