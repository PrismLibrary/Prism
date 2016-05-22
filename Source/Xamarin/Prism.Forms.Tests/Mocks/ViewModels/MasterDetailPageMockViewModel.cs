using System;
using Prism.Navigation;

namespace Prism.Forms.Tests.Mocks.ViewModels
{
    public class MasterDetailPageMockViewModel : ViewModelBase
    {
    }

    public class MasterDetailPageEmptyMockViewModel : ViewModelBase, IMasterDetailPageOptions
    {
        public bool IsPresentedAfterNavigation { get; set; }
    }
}
