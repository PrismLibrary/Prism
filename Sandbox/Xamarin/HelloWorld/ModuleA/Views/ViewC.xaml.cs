using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class ViewC : ContentPage, IDestroy
    {
        public ViewC()
        {
            InitializeComponent();
        }

        public void Destroy()
        {
            
        }
    }
}
