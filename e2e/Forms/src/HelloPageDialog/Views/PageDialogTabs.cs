using Xamarin.Forms;

namespace HelloPageDialog.Views
{
    public class PageDialogTabs : TabbedPage
    {
        public PageDialogTabs()
        {
            SetBinding(TitleProperty, new Binding("CurrentPage.Title", source: this));
        }
    }
}
