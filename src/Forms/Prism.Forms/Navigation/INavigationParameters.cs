using System.Collections.Generic;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for the INavigationService to pass parameteres during navigation.
    /// </summary>
    public interface INavigationParameters : IEnumerable<KeyValuePair<string, object>>
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
        /// The count, or number, of parameters in collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns an IEnumerable of the Keys in the collection
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Returns the value of the member referenced by key
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="key">The key for the value to be returned</param>
        /// <returns>Returns a matching parameter of <see cref="T" /> if one exists in the Collection</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Returns an IEnumerable of all parameters 
        /// </summary>
        /// <typeparam name="T">The type for the values to be returned</typeparam>
        /// <param name="key">The key for the values to be returned</param>
        ///<returns>Returns a IEnumberable of all the instances of type <see cref="T"/></returns>
        IEnumerable<T> GetValues<T>(string key);

        /// <summary>
        /// Checks to see if the parameter collection contains the value
        /// </summary>
        /// <typeparam name="T">The type for the values to be returned</typeparam>
        /// <param name="key">The key for the value to be returned</param>
        /// <param name="value">Value of the returned parameter if it exists</param>
        bool TryGetValue<T>(string key, out T value);

        //legacy
        object this[string key] { get; }
    }
}
