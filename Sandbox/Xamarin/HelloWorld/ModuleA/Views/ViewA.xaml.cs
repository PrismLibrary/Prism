using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class ViewA : ContentPage, IDestroy
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
