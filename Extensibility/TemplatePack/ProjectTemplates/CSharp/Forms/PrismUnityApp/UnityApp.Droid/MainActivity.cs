using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;

namespace $saferootprojectname$.Droid
{
    [Activity (Label = "$saferootprojectname$", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate( Bundle savedInstanceState )
        {
            TabLayoutResource = Resource.Layout.tabs;
            ToolbarResource = Resource.Layout.toolbar;

            base.OnCreate( savedInstanceState );

            global::Xamarin.Forms.Forms.Init( this, savedInstanceState );
            LoadApplication( new App () );
        }
    }
}

