﻿using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.DI.Forms.Tests.Mocks.ViewModels
{
    public class Issue2415PageViewModel : BindableBase, IInitialize
    {
        private readonly IRegionManager _regionManager;

        public Issue2415PageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public NavigationResult Result { get; private set; }

        public void Initialize(INavigationParameters parameters)
        {
            _regionManager.RequestNavigate("ContentRegion", "Issue2415RegionView", NavigationCallback);
        }

        private void NavigationCallback(NavigationResult result)
        {
            Result = result;
        }
    }

    public class Issue2415RegionViewModel
    {

    }
}
