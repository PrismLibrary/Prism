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
        }

        public void Destroy()
        {
            
        }
    }
}
