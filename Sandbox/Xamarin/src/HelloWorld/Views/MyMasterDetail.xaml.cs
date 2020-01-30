using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HelloWorld.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class MyMasterDetail : IMasterDetailPageOptions
    {
        public MyMasterDetail()
        {
            InitializeComponent();
        }

        public bool IsPresentedAfterNavigation => Device.Idiom != TargetIdiom.Phone;
    }
}
