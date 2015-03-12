using Microsoft.Practices.Unity;
using Prism.Unity;
using Xamarin.Forms;
using XamarinStore.Core.Services;
using XamarinStore.Views;

namespace XamarinStore
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override Page CreateMainPage()
        {
            return new ProductList();
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IStoreService, StoreService>(new ContainerControlledLifetimeManager());
        }
    }
}
