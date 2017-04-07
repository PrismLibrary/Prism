using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.ViewModels;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Forms.Tests.Mvvm
{
    public class ViewModelLocatorFixture
    {
        [Fact]
        public void ShouldLocateViewModelWithDefaultSettings()
        {
            ResetViewModelLocationProvider();

            PageMock view = new PageMock();
            Assert.Null(view.BindingContext);

            ViewModelLocator.SetAutowireViewModel(view, true);

            Assert.NotNull(view.BindingContext);
            Assert.IsType< PageMockViewModel>(view.BindingContext);
        }

        [Fact]
        public void GetAutowireViewModelShoudBeTrueWhenSet()
        {
            ResetViewModelLocationProvider();

            PageMock view = new PageMock();

            ViewModelLocator.SetAutowireViewModel(view, true);

            Assert.True(ViewModelLocator.GetAutowireViewModel(view));
        }

        [Fact]
        public void ShouldUseCustomDefaultViewModelFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            PageMock view = new PageMock();
            Assert.Null(view.BindingContext);

            object mockObject = new object();
            ViewModelLocationProvider.SetDefaultViewModelFactory(viewType => mockObject);

            ViewModelLocator.SetAutowireViewModel(view, true);
            Assert.NotNull(view.BindingContext);
            Assert.Equal(mockObject, view.BindingContext);
        }

        [Fact]
        public void ShouldUseCustomDefaultViewTypeToViewModelTypeResolverWhenSet()
        {
            ResetViewModelLocationProvider();

            PageMock view = new PageMock();
            Assert.Null(view.BindingContext);

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => typeof(ViewModelLocatorFixture));

            ViewModelLocator.SetAutowireViewModel(view, true);
            Assert.NotNull(view.BindingContext);
            Assert.IsType<ViewModelLocatorFixture>(view.BindingContext);
        }

        [Fact]
        public void ShouldUseCustomFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            PageMock view = new PageMock();
            Assert.Null(view.BindingContext);

            string viewModel = "Test String";
            ViewModelLocationProvider.Register(view.GetType().ToString(), () => viewModel);

            ViewModelLocator.SetAutowireViewModel(view, true);
            Assert.NotNull(view.BindingContext);
            Assert.Equal(viewModel, view.BindingContext);
        }

        private static void ResetViewModelLocationProvider()
        {
            TypeInfo staticType = typeof(ViewModelLocationProvider).GetTypeInfo();

            ConstructorInfo ci = null;
            foreach (var ctor in staticType.DeclaredConstructors)
            {
                ci = ctor;
                continue;
            }

            object[] parameters = new object[0];
            ci.Invoke(null, parameters);
        }
    }
}
