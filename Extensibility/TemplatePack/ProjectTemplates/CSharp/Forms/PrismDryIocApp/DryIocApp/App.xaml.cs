﻿using DryIoc;
using Prism.DryIoc;
using $safeprojectname$.Views;
using $safeprojectname$.ViewModels;

namespace $safeprojectname$
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("MainPage?title=Hello%20from%20Xamarin.Forms");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>();
            Container.Register<MainPageViewModel>(Reuse.Transient);
        }
    }
}
