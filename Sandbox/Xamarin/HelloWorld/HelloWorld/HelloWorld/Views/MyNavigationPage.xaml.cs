using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace HelloWorld.Views
{
    public partial class MyNavigationPage : NavigationPage, INavigationPageOptions, IDestroy
    {
        public MyNavigationPage()
        {
            InitializeComponent();
        }

        public bool ClearNavigationStackOnNavigation
        {
            get { return true; }
        }

        public void Destroy()
        {
            
        }
    }
}
