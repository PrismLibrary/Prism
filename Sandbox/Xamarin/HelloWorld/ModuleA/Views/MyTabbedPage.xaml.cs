using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace ModuleA.Views
{
    public partial class MyTabbedPage : TabbedPage, IDestroy
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
