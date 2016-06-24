using Prism.Unity;
using ${Namespace}.Views;

namespace ${Namespace}
{
	public partial class App : PrismApplication
	{
		public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
		{
			InitializeComponent();
		}

		protected override void OnInitialized()
		{
			NavigationService.NavigateAsync("MainPage?title=Hello%20from%20Xamarin.Forms");
		}

		protected override void RegisterTypes()
		{
			Container.RegisterTypeForNavigation<MainPage>();
		}
	}
}

