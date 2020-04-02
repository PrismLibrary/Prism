using System.Collections.Generic;

namespace Prism.Services.Dialogs
{
    //TODO: this can eventually be replaced with INavigationParameters
    public interface IDialogParameters
    {
        void Add(string key, object value);

        bool ContainsKey(string key);

        int Count { get; }

        IEnumerable<string> Keys { get; }

        T GetValue<T>(string key);

        IEnumerable<T> GetValues<T>(string key);

        bool TryGetValue<T>(string key, out T value);
    }
}
