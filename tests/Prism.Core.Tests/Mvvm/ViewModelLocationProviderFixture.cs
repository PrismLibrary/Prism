using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Prism.Tests.Mocks.ViewModels;
using Prism.Tests.Mocks.Views;
using Xunit;

namespace Prism.Tests.Mvvm
{
    public class ViewModelLocationProviderFixture
    {
        [Fact]
        public void ShouldLocateViewModelWithDefaultSettings()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<MockViewModel>(vm);
            });
        }

        [Fact]
        public void ShouldLocateViewModelWithDefaultSettingsForViewsThatEndWithView()
        {
            ResetViewModelLocationProvider();

            var view = new MockView();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<MockViewModel>(vm);
            });
        }

        [Fact]
        public void ShouldUseCustomDefaultViewModelFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            object mockObject = new object();
            ViewModelLocationProvider.SetDefaultViewModelFactory(viewType => mockObject);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType(mockObject.GetType(), vm);
            });
        }

        [Fact]
        public void ShouldUseCustomDefaultViewTypeToViewModelTypeResolverWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => typeof(ViewModelLocationProviderFixture));

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<ViewModelLocationProviderFixture>(vm);
            });
        }

        [Fact]
        public void ShouldFailWhenCustomDefaultViewTypeToViewModelTypeResolverIsNull()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => null);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.Null(vm);
            });
        }

        [Fact]
        public void ShouldUseCustomFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            string viewModel = "Test String";
            ViewModelLocationProvider.Register(view.GetType().ToString(), () => viewModel);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
                {
                    Assert.NotNull(v);
                    Assert.NotNull(vm);
                    Assert.Equal(viewModel, vm);
                });
        }

        [Fact]
        public void ShouldUseCustomFactoryWhenSet_Generic()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            string viewModel = "Test String";
            ViewModelLocationProvider.Register<Mock>(() => viewModel);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.Equal(viewModel, vm);
            });
        }

        [Fact]
        public void ShouldUseCustomTypeWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.Register(view.GetType().ToString(), typeof(ViewModelLocationProviderFixture));

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<ViewModelLocationProviderFixture>(vm);
            });
        }

        [Fact]
        public void ShouldUseCustomTypeWhenSet_Generic()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.Register<Mock, ViewModelLocationProviderFixture>();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<ViewModelLocationProviderFixture>(vm);
            });
        }

        private static void ResetViewModelLocationProvider() =>
            ViewModelLocationProvider.Reset();

        [Fact]
        public void NullTypeRegistrationPreservesConventionFallback()
        {
            ResetViewModelLocationProvider();
            ViewModelLocationProvider.Register(typeof(Mock).ToString(), (Type)null!);
            object actual = null;

            ViewModelLocationProvider.AutoWireViewModelChanged(new Mock(), (_, vm) => actual = vm);

            Assert.IsType<MockViewModel>(actual);
        }

        [Fact]
        public void PreservedViewModelResolvesByConventionWithoutTypeMapping()
        {
            ResetViewModelLocationProvider();
            ContainerAot.Preserve<MockViewModel>();
            object actual = null;

            ViewModelLocationProvider.AutoWireViewModelChanged(new Mock(), (_, vm) => actual = vm);

            Assert.IsType<MockViewModel>(actual);
        }

        [Fact]
        public void CustomFactoryWithViewTakesPrecedenceForPreservedViewModels()
        {
            ResetViewModelLocationProvider();
            ContainerAot.Preserve<MockViewModel>();
            var view = new Mock();
            var expected = new object();
            ViewModelLocationProvider.SetDefaultViewModelFactory(type => throw new InvalidOperationException("The view-aware factory should take precedence."));
            ViewModelLocationProvider.SetDefaultViewModelFactory((actualView, type) =>
            {
                Assert.Same(view, actualView);
                Assert.Equal(typeof(MockViewModel), type);
                return expected;
            });
            object actual = null;

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (_, vm) => actual = vm);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RegisteredViewModelWithoutDefaultConstructorUsesConfiguredFactory()
        {
            ResetViewModelLocationProvider();
            var dependency = new object();
            var expected = new MockViewModelWithDependency(dependency);
            ViewModelLocationProvider.Register<Mock, MockViewModelWithDependency>();
            ViewModelLocationProvider.SetDefaultViewModelFactory(type =>
            {
                Assert.Equal(typeof(MockViewModelWithDependency), type);
                return expected;
            });
            object actual = null;

            ViewModelLocationProvider.AutoWireViewModelChanged(new Mock(), (_, model) => actual = model);

            Assert.Same(expected, actual);
            Assert.Same(dependency, ((MockViewModelWithDependency)actual).Dependency);
        }

        [Fact]
        public void ReplacingOneMappingPreservesOtherMappingsForTheSameType()
        {
            ResetViewModelLocationProvider();
            ViewModelLocationProvider.Register<Mock, MockViewModel>();
            ViewModelLocationProvider.Register<MockView, MockViewModel>();
            ViewModelLocationProvider.Register<Mock, ViewModelLocationProviderFixture>();
            object actual = null;

            ViewModelLocationProvider.AutoWireViewModelChanged(new MockView(), (_, vm) => actual = vm);

            Assert.IsType<MockViewModel>(actual);
        }

        [Fact]
        public void ReplacingLastMappingReleasesCollectibleViewModelType()
        {
            ResetViewModelLocationProvider();
            var type = RegisterAndReplaceCollectibleViewModel();

            for (var attempt = 0; type.IsAlive && attempt < 10; attempt++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            Assert.False(type.IsAlive);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference RegisterAndReplaceCollectibleViewModel()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("CollectibleViewModels"), AssemblyBuilderAccess.RunAndCollect);
            var module = assembly.DefineDynamicModule("ViewModels");
            var builder = module.DefineType("CollectibleViewModel", TypeAttributes.Public);
            builder.DefineDefaultConstructor(MethodAttributes.Public);
            var type = builder.CreateTypeInfo().AsType();
            ViewModelLocationProvider.Register(typeof(Mock).ToString(), type);
            ViewModelLocationProvider.Register<Mock, MockViewModel>();
            return new WeakReference(type);
        }
    }
}
