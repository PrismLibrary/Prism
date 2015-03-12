using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamarinStore.Core.Business;

namespace XamarinStore.Core.Services
{
    public interface IStoreService
    {
        Task<List<Product>> GetProducts();
    }
}
