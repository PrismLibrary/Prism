using System.Collections.Generic;

namespace Prism.Navigation
{
    public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        void Add(string key, object value);

        bool ContainsKey(string key);

        int Count { get; }

        IEnumerable<string> Keys { get; }

        T GetValue<T>(string key);

        IEnumerable<T> GetValues<T>(string key);

        bool TryGetValue<T>(string key, out T value);

        //legacy
        object this[string key] { get; }
    }
}
