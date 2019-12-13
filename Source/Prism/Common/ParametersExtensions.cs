using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Prism.Common
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ParametersExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T GetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
            (T)GetValue(parameters, key, typeof(T));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetValue(this IEnumerable<KeyValuePair<string, object>> parameters, string key, Type type)
        {
            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    if(TryGetValueInternal(kvp, type, out var value))
                        return value;

                    throw new InvalidCastException($"Unable to convert the value of Type '{kvp.Value.GetType().FullName}' to '{type.FullName}' for the key '{key}' ");
                }
            }

            return GetDefault(type);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool TryGetValue<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key, out T value)
        {
            var type = typeof(T);

            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    var success = TryGetValueInternal(kvp, typeof(T), out object valueAsObject);
                    value = (T)valueAsObject;
                    return success;
                }
            }

            value = default;
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> GetValues<T>(this IEnumerable<KeyValuePair<string, object>> parameters, string key)
        {
            List<T> values = new List<T>();
            var type = typeof(T);

            foreach (var kvp in parameters)
            {
                if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                {
                    TryGetValueInternal(kvp, type, out var value);
                    values.Add((T)value);
                }
            }

            return values.AsEnumerable();
        }

        private static bool TryGetValueInternal(KeyValuePair<string, object> kvp, Type type, out object value)
        {
            value = GetDefault(type);
            var success = false;
            if (kvp.Value == null)
            {
                success = true;
            }
            else if (kvp.Value.GetType() == type)
            {
                success = true;
                value = kvp.Value;
            }
            else if (type.IsAssignableFrom(kvp.Value.GetType()))
            {
                success = true;
                value = kvp.Value;
            }
            else if (type.IsEnum)
            {
                var valueAsString = kvp.Value.ToString();
                if (Enum.IsDefined(type, valueAsString))
                {
                    success = true;
                    value = Enum.Parse(type, valueAsString);
                }
                else if (int.TryParse(valueAsString, out var numericValue))
                {
                    success = true;
                    value = Enum.ToObject(type, numericValue);
                }
            }

            if (!success && type.GetInterface("System.IConvertible") != null)
            {
                success = true;
                value = Convert.ChangeType(kvp.Value, type);
            }

            return success;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool ContainsKey(this IEnumerable<KeyValuePair<string, object>> parameters, string key) =>
            parameters.Any(x => string.Compare(x.Key, key, StringComparison.Ordinal) == 0);

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
