using System;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class MasterDetailPageMock : MasterDetailPage, IMasterDetailPageOptions
    {
        public MasterDetailPageMock()
        {
            Detail = new ContentPageMock();

            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        public bool IsPresentedAfterNavigation { get; set; }
    }

    public class MasterDetailPageEmptyMock : MasterDetailPage
    {
        public MasterDetailPageEmptyMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}
