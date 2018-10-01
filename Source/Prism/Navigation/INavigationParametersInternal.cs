namespace Prism.Navigation
{
    public interface INavigationParametersInternal
    {
        void Add(string key, object value);

        void Remove(string key);

        bool ContainsKey(string key);

        T GetValue<T>(string key);
    }
}
