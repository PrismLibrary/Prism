using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HelloWorld.ViewModels
{
    public class MyTabbedPageViewModel : ViewModelBase
    {
        string _title = "My Tabbed Page";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<MonkeyDataModel> _pages;
        public ObservableCollection<MonkeyDataModel> Pages
        {
            get { return _pages; }
            set { SetProperty(ref _pages, value); }
        }

        private MonkeyDataModel _selectedPage;
        public MonkeyDataModel SelectedPage
        {
            get { return _selectedPage; }
            set { SetProperty(ref _selectedPage, value); }
        } 

        public MyTabbedPageViewModel()
        {
            Pages = new ObservableCollection<MonkeyDataModel>(MonkeyDataModel.All);
        }

        public override void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            //var selectedItemName = (string)parameters["selectedItem"];

            //SelectedPage = Pages.Where(p => p.Name == selectedItemName).FirstOrDefault();
        }
    }

    public class MonkeyDataModel
    {
        static MonkeyDataModel()
        {
            All = new ObservableCollection<MonkeyDataModel>
            {
                new MonkeyDataModel
                {
                    Name = "Chimpanzee",
                    Family = "Hominidae",
                    Subfamily = "Homininae",
                    Tribe = "Panini",
                    Genus = "Pan",
                    PhotoUrl="http://upload.wikimedia.org/wikipedia/commons/thumb/6/62/Schimpanse_Zoo_Leipzig.jpg/640px-Schimpanse_Zoo_Leipzig.jpg"
                },
                new MonkeyDataModel
                {
                    Name = "Orangutan",
                    Family = "Hominidae",
                    Subfamily = "Ponginae",
                    Genus = "Pongo",
                    PhotoUrl="http://upload.wikimedia.org/wikipedia/commons/b/be/Orang_Utan%2C_Semenggok_Forest_Reserve%2C_Sarawak%2C_Borneo%2C_Malaysia.JPG"
                },
                new MonkeyDataModel
                {
                    Name = "Tamarin",
                    Family = "Callitrichidae",
                    Genus = "Saguinus",
                    PhotoUrl="http://upload.wikimedia.org/wikipedia/commons/thumb/8/85/Tamarin_portrait_2_edit3.jpg/640px-Tamarin_portrait_2_edit3.jpg"
                }
            };
        }

        public string Name { set; get; }

        public string Family { set; get; }

        public string Subfamily { set; get; }

        public string Tribe { set; get; }

        public string Genus { set; get; }

        public string PhotoUrl { set; get; }

        public static IList<MonkeyDataModel> All { set; get; }
    }
}
