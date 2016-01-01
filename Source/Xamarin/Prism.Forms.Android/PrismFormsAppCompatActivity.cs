using System;
using Android.App;
using Xamarin.Forms.Platform.Android;

namespace Prism.Forms.Android
{

	public class PrismFormsAppCompatActivity : FormsAppCompatActivity
	{
		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			OnBackButtonEvent ();

		}

		public event Action OnBackButtonEvent = delegate {};
	}
}
