using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class ViewC : ContentPage, IDestructible
    {
        public ViewC()
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
