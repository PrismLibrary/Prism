using System;
using Prism.Navigation;
using ModuleA.Models;
using ModuleA.Services;

namespace ModuleA.ViewModels
{
    public class ItemDetailPageViewModel : ViewModelBase, INavigationAware
    {
        private readonly MockDataStore _datastore;

        private Item _item;
        public Item Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ItemDetailPageViewModel()
        {
            _datastore = new MockDataStore();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.TryGetValue(nameof(Item), out Item item) && item != null)
                {
                    Item = item;
                    Title = item?.Text;
                }
                else if (parameters.TryGetValue("Id", out int itemId))
                {
                    var selectedItem = await _datastore.GetItemAsync(itemId);
                    Item = selectedItem;
                    Title = selectedItem?.Text;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
