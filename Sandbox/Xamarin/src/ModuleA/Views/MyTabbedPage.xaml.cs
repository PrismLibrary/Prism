using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class MyTabbedPage : TabbedPage, IDestructible
    {
        public MyTabbedPage()
        {
            InitializeComponent();
        }

        public void Destroy()
        {
         
        }
    }
}
