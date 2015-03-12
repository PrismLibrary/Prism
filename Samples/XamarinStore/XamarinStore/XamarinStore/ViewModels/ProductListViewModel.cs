using System.Collections.Generic;
using Prism.Mvvm;
using XamarinStore.Core.Business;
using XamarinStore.Core.Services;

namespace XamarinStore.ViewModels
{
    public class ProductListViewModel : BindableBase
    {
        private readonly IStoreService _service;

        private List<Product> _products;
        public List<Product> Products
        {
            get { return _products; }
            set { SetProperty(ref _products, value); }
        }

        public ProductListViewModel(IStoreService service)
        {
            _service = service;

            LoadProducts();
        }

        async void LoadProducts()
        {
            Products = await _service.GetProducts();
        }
    }
}
