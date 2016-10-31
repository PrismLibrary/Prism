using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HelloWorld.Views
{
    public partial class MainPage : ContentPage, IDestructible
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void Destroy()
        {
            
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            
        }
    }
}
