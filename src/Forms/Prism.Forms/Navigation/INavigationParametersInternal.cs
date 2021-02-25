namespace Prism.Navigation
{
    /// <summary>
    /// Used to set internal parameters used by Prism's <see cref="INavigationService"/>
    /// </summary>
    public interface INavigationParametersInternal
    {
        /// <summary>
        /// Adds the key and value to the parameters Collection
        /// </summary>
        /// <param name="key">The key to reference this value in the parameters collection.</param>
        /// <param name="value">The value of the parameter to store</param>
        void Add(string key, object value);

        /// <summary>
        /// Checks collection for presence of key
        /// </summary>
        /// <param name="key">The key to check in the Collection</param>
        /// <returns><c>true</c> if key exists; else returns <c>false</c>.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Returns the value of the member referenced by key
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="key">The key for the value to be returned</param>
        /// <returns>Returns a matching parameter of <typeparamref name="T"/> if one exists in the Collection</returns>
        T GetValue<T>(string key);
    }
}
