using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Prism.Events;
using HelloWorld.Events;
using Microsoft.Practices.ServiceLocation;

namespace HelloWorld.WinPhone
{
    public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new HelloWorld.App());

            IEventAggregator ea = ServiceLocator.Current.GetInstance<IEventAggregator>();
            ea.GetEvent<NavigationUriReceivedEvent>().Publish("android-app://HelloWorld/MyNavigationPage/ViewA?viewName=ViewA&id=1/ViewB?viewName=ViewB&id=2/ViewC?message=DeepLink&id=3");
            //ea.GetEvent<NavigationUriReceivedEvent>().Publish("android-app://HelloWorld/MyNavigationPage/ViewB/ViewC");
            //ea.GetEvent<NavigationUriReceivedEvent>().Publish("android-app://HelloWorld/ViewA/ViewB/ViewC");
        }
    }
}
