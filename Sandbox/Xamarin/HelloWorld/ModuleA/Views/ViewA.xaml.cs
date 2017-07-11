using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class ViewA : ContentPage, IDestructible
    {
        public ViewA()
        {
            InitializeComponent();
            //42548
            NavigationPage.SetHasBackButton(this, false);
        }

        public void Destroy()
        {
            
        }
    }
}
