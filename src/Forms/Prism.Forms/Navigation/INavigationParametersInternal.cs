namespace Prism.Navigation
{
    /// <summary>
    /// Internal - Provides a way for the INavigationService to pass parameteres during navigation.
    /// </summary>
    public interface INavigationParametersInternal
    {
        /// <summary>
        /// Adds the key and value to the KeyValuePair<string,object> collection
        /// </summary>
        /// <param name="key">The key to reference this value in the KeyValuePair<string, object></param>
        /// <param name="value">The value of the parameter to store</param>
        void Add(string key, object value);

        /// <summary>
        /// Checks collection for presense of key
        /// </summary>
        /// <param name="key">The key to check in the collection</param>
        /// <returns><c>true</c> if key exists; else returns <c>false</c>.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Returns the value of the member referenced by key
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="key">The key for the value to be returned</param>
        /// <returns>Returns a matching parameter of <see cref="T" /> if one exists in the Collection</returns>
        T GetValue<T>(string key);
    }
}
