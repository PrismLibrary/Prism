﻿using Prism.Unity;
using $safeprojectname$.Views;

namespace $safeprojectname$
{
    public partial class App : PrismApplication
    {
        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.Navigate("MainPage?title=Hello%20from%20Xamarin.Forms");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainPage>();
        }
    }
}
