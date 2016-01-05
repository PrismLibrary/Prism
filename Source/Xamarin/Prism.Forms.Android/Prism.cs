using System;
using Android.App;
using Xamarin.Forms.Platform.Android;

namespace Prism.Forms.Android
{
	public static class Prism
	{
		private static PrismApplicationBase _application;
		public static void Init(PrismFormsAppCompatActivity activity, PrismApplicationBase application)
		{
			_application = application;
			activity.OnBackButtonEvent += Activity_OnBackButtonEvent;
		}

		static void Activity_OnBackButtonEvent ()
		{
			_application.OnPlatformBackNavigation();
			
		}

	}

}

