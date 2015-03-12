using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XamarinStore.Core.Business;

namespace XamarinStore.Core.Services
{
    public class StoreService : IStoreService
    {
        List<Product> _products;
        public async Task<List<Product>> GetProducts()
        {
            if (_products == null)
            {
                using (var client = new HttpClient())
                {
                    string response = await client.GetStringAsync("https://xamarin-store-app.xamarin.com/api/products");
                    _products = JsonConvert.DeserializeObject<List<Product>>(response);
                }
            }
            return _products;
        }
    }
}
