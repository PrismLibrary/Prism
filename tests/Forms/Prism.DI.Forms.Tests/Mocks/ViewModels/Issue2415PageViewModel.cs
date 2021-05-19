using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
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

        public void Initialize(INavigationParameters parameters)
{
            _regionManager.RequestNavigate("ContentRegion", "Issue2415RegionView");
        }
    }

    public class Issue2415RegionViewModel
    {

    }
}
