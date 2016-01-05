using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Prism.Forms.Android;

namespace HelloWorld.Droid
{
    [Activity(Label = "HelloWorld", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : PrismFormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.tabs;
            ToolbarResource = Resource.Layout.toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
			var app = new App ();
			Prism.Forms.Android.Prism.Init(this, app);

            LoadApplication(app);
        }

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();

		}
    }
}

