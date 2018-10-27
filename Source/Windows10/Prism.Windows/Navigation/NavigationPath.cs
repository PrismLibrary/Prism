using System;
using System.Linq;
using Windows.Foundation;

namespace Prism.Navigation
{
    public class NavigationPath : INavigationPath
    {
        private readonly string _originalString;

        public string QueryString { get; }

        public NavigationPath(int index, string originalString, INavigationParameters parameters)
        {
            Index = index;

            _originalString = originalString;

            // parse name/key

            Key = originalString.Split('?').First();

            // parse query

            var queryString = originalString.Split('?').Last();
            if (queryString != Key)
            {
                QueryString = queryString;
            }

            // parse parameters

            if (!string.IsNullOrEmpty(QueryString))
            {
                var query = new WwwFormUrlDecoder(QueryString);
                foreach (var item in query)
                {
                    Parameters.Add(item.Name, item.Value);
                }
            }

            // merge parameters

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    Parameters.Add(item.Key, item.Value);
                }
            }

            // get types

            if (PageRegistry.TryGetRegistration(Key, out var info))
            {
                Key = info.Key;
                View = info.View;
                ViewModel = info.ViewModel;
            }
            else
            {
                throw new Exception($"Count not find Key:[{Key}]. Use RegisterTypes()/container.RegisterForNavigation<T, T>().");
            }
        }

        public int Index { get; }

        public INavigationParameters Parameters { get; } = new NavigationParameters();

        public string Key { get; }

        public Type View { get; }

        public Type ViewModel { get; }

        public override string ToString()
        {
            return _originalString;
        }
    }
}
