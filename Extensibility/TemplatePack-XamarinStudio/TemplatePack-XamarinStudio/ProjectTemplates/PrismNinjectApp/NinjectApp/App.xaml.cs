using Prism.Ninject;
using ${Namespace}.Views;

namespace ${Namespace}
{
	public partial class App : PrismApplication
	{
		public App( IPlatformInitializer initializer = null ) :
			base( initializer )
		{
			InitializeComponent();
		}

		protected async override void OnInitialized()
		{
			await NavigationService.NavigateAsync("MainPage?title=Hello%20from%20Xamarin.Forms");
		}

		protected override void RegisterTypes()
		{
			Container.RegisterTypeForNavigation<MainPage>();
		}
	}
}

