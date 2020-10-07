using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Regions.Behaviors
{
    /// <summary>
    /// Defines a factory that allows the registration of the default set of <see cref="IRegionBehavior"/>, that will
    /// be added to the <see cref="IRegionBehaviorCollection"/> of all <see cref="IRegion"/>s, unless overridden on a 'per-region' basis.
    /// </summary>
    public class RegionBehaviorFactory : IRegionBehaviorFactory
    {
        private readonly IContainerProvider _container;
        private readonly Dictionary<string, Type> _registeredBehaviors = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of <see cref="RegionBehaviorFactory"/>.
        /// </summary>
        /// <param name="container"><see cref="IContainerExtension"/> used to create the instance of the behavior from its <see cref="Type"/>.</param>
        public RegionBehaviorFactory(IContainerExtension container)
        {
            _container = container;
        }

        /// <summary>
        /// Adds a particular type of RegionBehavior if it was not already registered. The <paramref name="behaviorKey"/> string is used to check if the behavior is already present
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior to add.</typeparam>
        /// <param name="behaviorKey">The behavior key that's used to find if a certain behavior is already added.</param>
        public void AddIfMissing<TBehavior>(string behaviorKey)
            where TBehavior : IRegionBehavior
        {
            if (behaviorKey == null)
            {
                throw new ArgumentNullException(nameof(behaviorKey));
            }

            var behaviorType = typeof(TBehavior);

            // Only add the behaviorKey if it doesn't already exists.
            if (_registeredBehaviors.ContainsKey(behaviorKey))
            {
                return;
            }

            _registeredBehaviors.Add(behaviorKey, behaviorType);
        }

        /// <summary>
        /// Creates an instance of the behavior <see cref="Type"/> that is registered using the specified key.
        /// </summary>
        /// <param name="key">The key that is used to register a behavior type.</param>
        /// <returns>A new instance of the behavior. </returns>
        public IRegionBehavior CreateFromKey(string key)
        {
            if (!ContainsKey(key))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentUICulture, Resources.TypeWithKeyNotRegistered, key), nameof(key));
            }

            return (IRegionBehavior)_container.Resolve(_registeredBehaviors[key]);
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator() =>
            _registeredBehaviors.Keys.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Determines whether a behavior with the specified key already exists.
        /// </summary>
        /// <param name="behaviorKey">The behavior key.</param>
        /// <returns>
        /// <see langword="true"/> if a behavior with the specified key is present; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(string behaviorKey) =>
            _registeredBehaviors.ContainsKey(behaviorKey);
    }
}
