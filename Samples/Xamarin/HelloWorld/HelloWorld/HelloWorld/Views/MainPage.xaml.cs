using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HelloWorld.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            //a navigation deep link can't be added without showing the transitions to the pages
            var p1 = new ViewA();
            var p2 = new ViewB();
            var p3 = new ViewC();
            var p4 = new MyTabbedPage();

            p1.Navigation.PushModalAsync(p2, false);
            p2.Navigation.PushModalAsync(p3, false);
            p3.Navigation.PushModalAsync(p4, false);

            Navigation.PushModalAsync(p1, false);
        }
    }
}
