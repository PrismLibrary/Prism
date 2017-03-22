#if TEST

using System;
using System.Collections.Generic;

namespace Prism
{
    public class FormsDependencyService : Dictionary<Type, object>
    {
        private FormsDependencyService()
        {
            
        }

        private static Lazy<FormsDependencyService> current =
            new Lazy<FormsDependencyService>(() => new FormsDependencyService());
        private static FormsDependencyService Current
        {
            get { return current.Value; }
        }

        public static T Get<T>(Xamarin.Forms.DependencyFetchTarget fetchTarget = Xamarin.Forms.DependencyFetchTarget.GlobalInstance)
        {
            var typeToResolve = typeof(T);
            return Current.ContainsKey(typeToResolve) ? (T)Current[typeToResolve] : default(T);
        }

        public static void Register<T>(T instance)
        {
            Current[typeof(T)] = instance;
        }
    }
}

#endif
