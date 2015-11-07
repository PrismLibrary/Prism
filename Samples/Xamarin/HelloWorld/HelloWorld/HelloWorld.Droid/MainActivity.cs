using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Prism.Events;
using Microsoft.Practices.ServiceLocation;
using HelloWorld.Events;

namespace HelloWorld.Droid
{
    [Activity(Label = "HelloWorld", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            IEventAggregator ea = ServiceLocator.Current.GetInstance<IEventAggregator>();
            //ea.GetEvent<NavigationUriReceivedEvent>().Publish("android-app://HelloWorld/ViewA?viewName=ViewA&id=1/ViewB?viewName=ViewB&id=2/ViewC?message=DeepLink&id=3");
            //ea.GetEvent<NavigationUriReceivedEvent>().Publish("android-app://HelloWorld/ViewA/ViewB/ViewC");
        }
    }
}

