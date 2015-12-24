using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace $safeprojectname$
{
	public partial class MainPage : global::Xamarin.Forms.Platform.WinPhone.FormsApplicationPage
	{
		public MainPage ()
		{
			InitializeComponent();
			SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

			global::Xamarin.Forms.Forms.Init ();
			LoadApplication (new $saferootprojectname$.App ());
		}
	}
}
