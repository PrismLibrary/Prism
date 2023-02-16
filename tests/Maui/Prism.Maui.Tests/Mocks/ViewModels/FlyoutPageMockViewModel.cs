using System;
using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.ViewModels
{
    public class FlyoutPageMockViewModel : ViewModelBase
    {
    }

    public class FlyoutPageEmptyMockViewModel : ViewModelBase, IFlyoutPageOptions
    {
        public bool IsPresentedAfterNavigation { get; set; }
    }
}
