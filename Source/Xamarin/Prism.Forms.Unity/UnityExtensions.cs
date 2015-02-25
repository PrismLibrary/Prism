using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace Prism.Unity
{
    public static class UnityExtensions
    {
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name) where T : Page
        {
            container.RegisterType(typeof(object), typeof(T), name);
        }
    }
}
