using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class ViewB : ContentPage, IDestroy
    {
        public ViewB()
        {
            InitializeComponent();
        }

        public void Destroy()
        {
            
        }
    }
}
