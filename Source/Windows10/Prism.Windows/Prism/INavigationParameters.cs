using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Prism.Navigation
{
    public interface INavigationParametersInteral
    {
        void Add(string key, object value);

        T GetValue<T>(string key);
        bool TryGetValue<T>(string key, out T value);

        IEnumerable<T> GetValues<T>(string key);
        bool TryGetValues<T>(string key, out IEnumerable<T> values);
    }

    public interface INavigationParameters
    {
        void Add(string key, object value);

        IEnumerable<string> Keys { get; }
        bool ContainsKey(string key);

        T GetValue<T>(string key);
        bool TryGetValue<T>(string key, out T value);

        IEnumerable<T> GetValues<T>(string key);
        bool TryGetValues<T>(string key, out IEnumerable<T> values);

        int Count { get; }
        object this[string key] { get; }
    }

}