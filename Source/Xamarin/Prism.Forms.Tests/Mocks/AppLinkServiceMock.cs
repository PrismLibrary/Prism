using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Services;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class AppLinkServiceMock : IAppLinkService
    {
        public List<IAppLinkEntry> Entries { get; set; } = new List<IAppLinkEntry>();

        public void RegisterLink(IAppLinkEntry entry)
        {
            this.Entries.Add(entry);
        }

        public void DeregisterLink(IAppLinkEntry entry)
        {
            this.Entries.Remove(entry);
        }

        public void DeregisterLink(Uri uri)
        {
            var link = this.Entries.FirstOrDefault(x => x.AppLinkUri.Equals(uri));
            if (link != null)
                this.Entries.Remove(link);
        }
    }
}
