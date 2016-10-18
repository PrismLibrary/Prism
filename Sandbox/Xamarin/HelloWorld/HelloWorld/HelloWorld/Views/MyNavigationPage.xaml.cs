using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace HelloWorld.Views
{
    public partial class MyNavigationPage : NavigationPage, INavigationPageOptions, IDisposable
    {
        public MyNavigationPage()
        {
            InitializeComponent();
        }

        public bool ClearNavigationStackOnNavigation
        {
            get { return false; }
        }

        public void Dispose()
        {
            
        }
    }
}
