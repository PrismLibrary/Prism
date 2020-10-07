using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prism.Ioc
{
    /// <summary>
    /// A collection of Errors encountered by while attempting to resolve a given type.
    /// </summary>
    public sealed class ContainerResolutionErrorCollection : IEnumerable<KeyValuePair<Type, Exception>>
    {
        private readonly List<KeyValuePair<Type, Exception>> _errors = new List<KeyValuePair<Type, Exception>>();

        internal void Add(Type type, Exception exception) =>
            _errors.Add(new KeyValuePair<Type, Exception>(type, exception));

        /// <summary>
        /// Provides a list of <see cref="Type" />'s affected.
        /// </summary>
        /// <remarks>
        /// This could include <see cref="object"/> for Registered View's
        /// </remarks>
        public IEnumerable<Type> Types => _errors.Select(x => x.Key).Distinct();

        IEnumerator<KeyValuePair<Type, Exception>> IEnumerable<KeyValuePair<Type, Exception>>.GetEnumerator() =>
            _errors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _errors.GetEnumerator();
    }
}
