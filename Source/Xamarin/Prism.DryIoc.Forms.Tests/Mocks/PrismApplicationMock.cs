using System;
using System.Collections.Generic;
using DryIoc;
using Prism.DryIoc.Forms.Tests.Mocks.Modules;
using Prism.DryIoc.Forms.Tests.Mocks.Services;
using Prism.DryIoc.Forms.Tests.Mocks.ViewModels;
using Prism.DryIoc.Forms.Tests.Mocks.Views;
using Prism.Modularity;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.DryIoc.Forms.Tests.Mocks
{
    public class PrismApplicationMock : PrismApplication
    {
        public PrismApplicationMock()
        {
            NavigationServices = new Dictionary<Type, INavigationService>();
        }

        public PrismApplicationMock(Page startPage) : this()
        {
            Current.MainPage = startPage;
        }

        public IDictionary<Type, INavigationService> NavigationServices { get; }

        public new INavigationService NavigationService => base.NavigationService;

        public bool Initialized { get; private set; }

        protected override void OnInitialized()
        {
            Initialized = true;
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog.AddModule(new ModuleInfo
            {
                InitializationMode = InitializationMode.WhenAvailable,
                ModuleName = "ModuleMock",
                ModuleType = typeof(ModuleMock)
            });
        }

        protected override void RegisterTypes()
        {
            Container.Register<IDryIocServiceMock, DryIocServiceMock>();
            Container.RegisterTypeForNavigation<ViewMock>("view");
            Container.RegisterTypeForNavigation<ViewAMock, ViewModelAMock>();
            Container.Register<ViewModelAMock>();
            Container.Register<ViewModelBMock>(serviceKey: ViewModelBMock.Key);
            Container.Register<ConstructorArgumentViewModel>();
            Container.RegisterTypeForNavigation<AutowireView, AutowireViewModel>();
            Container.RegisterTypeForNavigation<ConstructorArgumentView, ConstructorArgumentViewModel>();
            Container.Register<ModuleMock>(Reuse.Singleton);
        }

        protected override void ResolveTypeForPage(Page view, Type type, INavigationService navigationService)
        {
            NavigationServices[type] = navigationService;
        }
    }
}