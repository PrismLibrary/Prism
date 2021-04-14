using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

#if HAS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace HelloUnoWorld.Views
{
	public sealed partial class InitialView : UserControl
	{
		public InitialView()
		{
			this.InitializeComponent();
		}
	}
}
