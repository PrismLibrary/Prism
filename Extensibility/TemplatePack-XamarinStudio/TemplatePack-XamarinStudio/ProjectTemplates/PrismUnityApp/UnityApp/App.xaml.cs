﻿using Prism.Unity;
using ${Namespace}.Views;

namespace ${Namespace}
{
	public partial class App : PrismApplication
	{
		protected override void OnInitialized()
		{
			InitializeComponent();

			NavigationService.NavigateAsync("MainPage?title=Hello%20from%20Xamarin.Forms");
		}

		protected override void RegisterTypes()
		{
			Container.RegisterTypeForNavigation<MainPage>();
		}
	}
}

