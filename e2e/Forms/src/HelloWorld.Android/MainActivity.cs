using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;

namespace HelloWorld.Droid
{
    [Activity(
        Theme = "@style/MainTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));
        }

        public override async void OnBackPressed()
        {
            var result = await PrismPlatform.OnBackPressed(this);
            if (!result.Success)
            {
                System.Diagnostics.Debugger.Break();
                if (result.Exception != null)
                {
                    Console.WriteLine(result.Exception);
                }
            }
        }
    }
}

