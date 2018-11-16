using System;
using Xamarin.Forms;

namespace Prism.Services
{
    public class AppLinkService : IAppLinkService
    {
        public void RegisterLink(IAppLinkEntry entry)
        {
            Application.Current.AppLinks.RegisterLink(entry);
        }

        public void DeregisterLink(IAppLinkEntry entry)
        {
            Application.Current.AppLinks.DeregisterLink(entry);
        }

        public void DeregisterLink(Uri uri)
        {
            Application.Current.AppLinks.DeregisterLink(uri);
        }
    }
}
