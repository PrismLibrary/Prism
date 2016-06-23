using Prism.Ninject;
using $safeprojectname$.Views;

namespace $safeprojectname$
{
    public partial class App : PrismApplication
    {
        protected async override void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("MainPage?title=Hello%20from%20Xamarin.Forms");
        }

        protected override void RegisterTypes()
        {
            Kernel.RegisterTypeForNavigation<MainPage>();
        }
    }
}
