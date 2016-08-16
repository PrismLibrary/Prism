using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Thrown when the navigation parameter can't be cast.
    /// </summary>
    public class NavigationParameterInvalidCastException : InvalidCastException
    {
        /// <summary>
        /// Creates an exception with the missing key and a default message about
        /// the missing key.
        /// </summary>
        /// <param name="invalidCastKey">The missing navigation parameters key.</param>
        /// <param name="expectedType">The expected type of the navigation parameter.</param>
        /// <param name="actualType">The actual type of the navigation parameter value.</param>
        public NavigationParameterInvalidCastException(string invalidCastKey, Type expectedType, Type actualType)
            : base("The navigation parameter value for key '" + invalidCastKey + "' is of type '" + actualType + "' and can't be cast to '" + expectedType + "'.")
        {
            if (invalidCastKey == null) throw new ArgumentNullException(nameof(invalidCastKey));
            if (expectedType == null) throw new ArgumentNullException(nameof(expectedType));
            if (actualType == null) throw new ArgumentNullException(nameof(actualType));

            InvalidCaskKey = invalidCastKey;
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        /// <summary>
        /// The key with the invalid cast.
        /// </summary>
        public string InvalidCaskKey { get; }

        /// <summary>
        /// The expected type of the navigation parameter.
        /// </summary>
        public Type ExpectedType { get; }

        /// <summary>
        /// The actual type of the navigation parameter value.
        /// </summary>s
        public Type ActualType { get; }
    }
}