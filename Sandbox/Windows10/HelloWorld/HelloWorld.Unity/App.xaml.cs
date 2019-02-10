using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using Sample.ViewModels;
using Sample.Views;
using SampleData.StarTrek;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sample
{
    sealed partial class App : PrismApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDatabase, Database>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ItemPage, ItemPageViewModel>();
        }

        protected override void OnStart(StartArgs args)
        {
            if (args.StartKind == StartKinds.Launch)
            {
                NavigationService.NavigateAsync("/MainPage");
            }
            else
            {
                // TODO
            }
        }
    }
}
