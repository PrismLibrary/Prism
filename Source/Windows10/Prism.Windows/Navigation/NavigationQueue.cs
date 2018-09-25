using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Navigation
{
    public class NavigationQueue : Queue<INavigationPath>
    {
        public NavigationQueue(IEnumerable<INavigationPath> collection)
            : base(collection.OrderBy(x => x.Index))
        {
            // empty
        }

        public bool ClearBackStack { get; set; }

        public override string ToString()
        {
            var prefix = ClearBackStack ? "/" : string.Empty;
            return $"{prefix}{string.Join("/", ToArray().Select(x => x.ToString()))}";
        }

        public static NavigationQueue Parse(string path, INavigationParameters parameters)
           => TryParse(path, parameters, out var queue) ? queue : throw new Exception();

        public static NavigationQueue Parse(Uri path, INavigationParameters parameters)
            => TryParse(path, parameters, out var queue) ? queue : throw new Exception();

        public static bool TryParse(string path, INavigationParameters parameters, out NavigationQueue queue)
        {
            if (string.IsNullOrEmpty(path))
            {
                queue = null;
                return false;
            }

            if (Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri))
            {
                return TryParse(new Uri(path, UriKind.Relative), parameters, out queue);
            }
            else
            {
                queue = null;
                return false;
            }
        }

        public static bool TryParse(Uri uri, INavigationParameters parameters, out NavigationQueue queue)
        {
            if (uri == null)
            {
                queue = null;
                return false;
            }

            if (uri.IsAbsoluteUri)
            {
                throw new Exception("Navigation path must not be absolute Uri.");
            }

            var groups = uri.OriginalString.Split('/')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select((path, index) => new NavigationPath(index, path, parameters));

            queue = new NavigationQueue(groups)
            {
                ClearBackStack = uri.OriginalString.StartsWith("/"),
            };

            return queue.Any();
        }
    }
}
