using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ModuleA.Views;
using Prism.Commands;
using Prism.Navigation;
using ModuleA.Models;
using ModuleA.Services;
using Xamarin.Forms;
using NavigationParameters = Prism.Navigation.NavigationParameters;
using Prism;
using Prism.AppModel;

namespace ModuleA.ViewModels
{
    public class ItemListPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly MockDataStore _datastore;
        public ObservableCollection<Item> Items { get; }= new ObservableCollection<Item>();
        public DelegateCommand LoadItemsCommand { get; }
        public DelegateCommand AddItemCommand { get; }
        public DelegateCommand<Item> ItemSelectedCommand { get; }
        public ItemListPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            LoadItemsCommand = new DelegateCommand(OnLoadItems);
            ItemSelectedCommand = new DelegateCommand<Item>(OnItemSelected);
            _datastore = new MockDataStore();
        }

        private async void OnLoadItems()
        {
            try
            {
                Items.Clear();
                var items = await _datastore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async void OnItemSelected(Item item)
        {
            var result = await _navigationService.NavigateAsync(nameof(ItemDetailPage),
                new NavigationParameters {{nameof(Item), item}});
            if (!result.Success)
            {
                Console.WriteLine(result.Exception);
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            OnLoadItems();
        }
    }
}