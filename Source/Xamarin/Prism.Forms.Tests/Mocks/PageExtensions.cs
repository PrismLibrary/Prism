using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public static class PageExtensions
    {
        public static void SetInner(this Page page, INavigation navigation)
        {
            var propertyInfo = page.Navigation.GetType().GetRuntimeProperty("Inner");
            propertyInfo.SetValue(page.Navigation, navigation);
        }

    }
}
