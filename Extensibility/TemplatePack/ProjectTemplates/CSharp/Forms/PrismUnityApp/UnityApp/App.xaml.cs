using Prism.Unity;
using $safeprojectname$.Views;

namespace $safeprojectname$
{
    public partial class App : PrismApplication
    {
        public App()
        {
            InitializeComponent();

            NavigationService.Navigate("MainNavigationPage/ViewA?message=Hello%20From%20Xamarin.Forms");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainNavigationPage>();
            Container.RegisterTypeForNavigation<ViewA>();
            Container.RegisterTypeForNavigation<ViewB>();
        }
    }
}
