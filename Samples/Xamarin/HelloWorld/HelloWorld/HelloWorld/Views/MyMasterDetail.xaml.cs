using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace HelloWorld.Views
{
    public partial class MyMasterDetail : MasterDetailPage
    {
        public MyMasterDetail()
        {
            InitializeComponent();

            Detail = new ViewA();

            //Label header = new Label
            //{
            //    Text = "MasterDetailPage",
            //    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            //    HorizontalOptions = LayoutOptions.Center
            //};

            //// Assemble an array of NamedColor objects.
            //NamedColor[] namedColors =
            //    {
            //        new NamedColor("Aqua", Color.Aqua),
            //        new NamedColor("Black", Color.Black),
            //        new NamedColor("Blue", Color.Blue),
            //        new NamedColor("Fuschia", Color.Fuschia),
            //        new NamedColor("Gray", Color.Gray),
            //        new NamedColor("Green", Color.Green),
            //        new NamedColor("Lime", Color.Lime),
            //        new NamedColor("Maroon", Color.Maroon),
            //        new NamedColor("Navy", Color.Navy),
            //        new NamedColor("Olive", Color.Olive),
            //        new NamedColor("Purple", Color.Purple),
            //        new NamedColor("Red", Color.Red),
            //        new NamedColor("Silver", Color.Silver),
            //        new NamedColor("Teal", Color.Teal),
            //        new NamedColor("White", Color.White),
            //        new NamedColor("Yellow", Color.Yellow)
            //    };

            //// Create ListView for the master page.
            //ListView listView = new ListView
            //{
            //    ItemsSource = namedColors
            //};

            //// Create the master page with the ListView.
            //this.Master = new ContentPage
            //{
            //    Title = header.Text,
            //    Content = new StackLayout
            //    {
            //        Children =
            //        {
            //            header,
            //            listView
            //        }
            //    }
            //};

            ////this.Detail = new ViewA();

            //// Initialize the ListView selection.
            //listView.SelectedItem = namedColors[0];

        }
    }

    class NamedColor
    {
        public NamedColor(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        public string Name { private set; get; }

        public Color Color { private set; get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
