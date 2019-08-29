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
                    if (kvp.Value == null)
                        return GetDefault(type);
                    else if (kvp.Value.GetType() == type)
                        return kvp.Value;
                    else if (type.IsAssignableFrom(kvp.Value.GetType()))
                        return kvp.Value;
                    else if (type.IsEnum && Enum.IsDefined(type, kvp.Value))
                        return Enum.Parse(type, kvp.Value.ToString(), true);
                    else
                        return Convert.ChangeType(kvp.Value, type);
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
                    if (kvp.Value == null)
                        value = default;
                    else if (kvp.Value.GetType() == type)
                        value = (T)kvp.Value;
                    else if (type.IsAssignableFrom(kvp.Value.GetType()))
                        value = (T)kvp.Value;
                    else if (type.IsEnum && Enum.TryParse<T>(kvp.Value, true, out var enumValue))
                        value = enumValue;
                    else
                        value = (T)Convert.ChangeType(kvp.Value, type);

                    return true;
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
                    if (kvp.Value == null)
                        values.Add(default);
                    else if (kvp.Value.GetType() == type)
                        values.Add((T)kvp.Value);
                    else if (type.IsAssignableFrom(kvp.Value.GetType()))
                        values.Add((T)kvp.Value);
                    else if (type.IsEnum && Enum.TryParse<T>(kvp.Value, true, out var enumValue))
                        value.Add(enumValue);
                    else
                        values.Add((T)Convert.ChangeType(kvp.Value, type));
                }
            }

            return values.AsEnumerable();
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
