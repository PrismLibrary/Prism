using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Mvvm;
using System.Reflection;
using Prism.Tests.Mocks.Views;
using Prism.Tests.Mocks.ViewModels;

namespace Prism.Tests.Mvvm
{
    [TestClass]
    public class ViewModelLocationProviderFixture
    {
        [TestMethod]
        public void ShouldLocateViewModelWithDefaultSettings()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.IsNotNull(v);
                Assert.IsNotNull(vm);
                Assert.IsInstanceOfType(vm, typeof(MockViewModel));
            });
        }

        [TestMethod]
        public void ShouldLocateViewModelWithDefaultSettingsForViewsThatEndWithView()
        {
            ResetViewModelLocationProvider();

            var view = new MockView();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.IsNotNull(v);
                Assert.IsNotNull(vm);
                Assert.IsInstanceOfType(vm, typeof(MockViewModel));
            });
        }

        [TestMethod]
        public void ShouldUseCustomDefaultViewModelFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            object mockObject = new object();
            ViewModelLocationProvider.SetDefaultViewModelFactory(viewType => mockObject);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.IsNotNull(v);
                Assert.IsNotNull(vm);
                Assert.IsInstanceOfType(vm, mockObject.GetType());
            }); 
        }

        [TestMethod]
        public void ShouldUseCustomDefaultViewTypeToViewModelTypeResolverWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => typeof(ViewModelLocationProviderFixture));

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.IsNotNull(v);
                Assert.IsNotNull(vm);
                Assert.IsInstanceOfType(vm, typeof(ViewModelLocationProviderFixture));
            });            
        }

        [TestMethod]
        public void ShouldFailWhenCustomDefaultViewTypeToViewModelTypeResolverIsNull()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => null);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.IsNotNull(v);
                Assert.IsNull(vm);
            });
        }

        [TestMethod]
        public void ShouldUseCustomFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            string viewModel = "Test String";
            ViewModelLocationProvider.Register(view.GetType().ToString(), () => viewModel);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
                {
                    Assert.IsNotNull(v);
                    Assert.IsNotNull(vm);
                    Assert.AreEqual(viewModel, vm);
                });
        }

        private static void ResetViewModelLocationProvider()
        {
            Type staticType = typeof(ViewModelLocationProvider);
            ConstructorInfo ci = staticType.TypeInitializer;
            object[] parameters = new object[0];
            ci.Invoke(null, parameters);
        }
    }
}
