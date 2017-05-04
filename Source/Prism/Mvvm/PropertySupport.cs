

using System;
using System.Linq.Expressions;
using System.Reflection;
using Prism.Properties;

namespace Prism.Mvvm
{
    ///<summary>
    /// Provides support for extracting property information based on a property expression.
    ///</summary>
    public static class PropertySupport
    {
        /// <summary>
        /// Extracts the property name from a property expression.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression (e.g. p => p.PropertyName)</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     Not a <see cref="MemberExpression"/><br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        ///     Or, the property is static.
        /// </exception>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            return ExtractPropertyNameFromLambda(propertyExpression);
        }

        /// <summary>
        /// Extracts the property name from a LambdaExpression.
        /// </summary>
        /// <param name="expression">The LambdaExpression</param>
        /// <returns>The name of the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="expression"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the expression is:<br/>
        ///     The <see cref="MemberExpression"/> does not represent a property.<br/>
        ///     Or, the property is static.
        /// </exception>
        internal static string ExtractPropertyNameFromLambda(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(Resources.PropertySupport_NotMemberAccessExpression_Exception, nameof(expression));

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException(Resources.PropertySupport_ExpressionNotProperty_Exception, nameof(expression));

            var getMethod = property.GetMethod;
            if (getMethod.IsStatic)
                throw new ArgumentException(Resources.PropertySupport_StaticExpression_Exception, nameof(expression));

            return memberExpression.Member.Name;
        }

    }
}
