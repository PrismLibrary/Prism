namespace Prism.Composition.Windows.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Policies;

    /// <summary>
    /// This strategy implements the logic that will return all instances
    /// when an <see cref="IEnumerable{T}"/> parameter is detected.
    /// </summary>
    public class EnumerableResolutionStrategy : BuilderStrategy
    {
        private static readonly MethodInfo GenericResolveEnumerableMethod =
            typeof(EnumerableResolutionStrategy).GetMethod("ResolveEnumerable", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private static readonly MethodInfo GenericResolveLazyEnumerableMethod =
            typeof(EnumerableResolutionStrategy).GetMethod("ResolveLazyEnumerable", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private delegate object Resolver(IBuilderContext context);

        /// <summary>
        /// Do the PreBuildUp stage of construction. This is where the actual work is performed.
        /// </summary>
        /// <param name="context">Current build context.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (IsResolvingIEnumerable(context.BuildKey.Type))
            {
                MethodInfo resolverMethod;
                Type typeToBuild = GetTypeToBuild(context.BuildKey.Type);

                if (IsResolvingLazy(typeToBuild))
                {
                    typeToBuild = GetTypeToBuild(typeToBuild);
                    resolverMethod = GenericResolveLazyEnumerableMethod.MakeGenericMethod(typeToBuild);
                }
                else
                {
                    resolverMethod = GenericResolveEnumerableMethod.MakeGenericMethod(typeToBuild);
                }

                var resolver = (Resolver) resolverMethod.CreateDelegate(typeof(Resolver), null);
                context.Existing = resolver(context);
                context.BuildComplete = true;
            }
        }

        private static Type GetTypeToBuild(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        private static bool IsResolvingIEnumerable(Type type)
        {
            return type.GetTypeInfo().IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsResolvingLazy(Type type)
        {
            return type.GetTypeInfo().IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Lazy<>);
        }

        private static object ResolveLazyEnumerable<T>(IBuilderContext context)
        {
            var container = context.NewBuildUp<IUnityContainer>();
            var typeToBuild = typeof(T);
            var typeWrapper = typeof(Lazy<T>);
            var results = ResolveAll(container, typeToBuild, typeWrapper).OfType<Lazy<T>>().ToList();

            var hashSet = new HashSet<Lazy<T>>();

            // Add parts from the MEF
            var containerPolicy = context.Policies.Get<ICompositionContainerPolicy>(null);
            if (containerPolicy != null)
            {
                var parts = ResolveAll(
                    containerPolicy.CompositionHostWithDefaultProviders,
                    typeWrapper,
                    null);

                results.AddRange(parts.Select(innerPart => (Lazy<T>)innerPart));
            }

            results.ForEach((result) => hashSet.Add(result));

            return results;
        }

        private static IEnumerable<object> ResolveAll(CompositionHost exportProvider, Type type, string name)
        {
            var exports = exportProvider.GetExports(type, name);

            if (exports.Count() == 0)
            {
                return Enumerable.Empty<object>();
            }

            var list = new List<object>();

            foreach (var export in exports)
            {
                list.Add(export);
            }

            return list;
        }

        private static object ResolveEnumerable<T>(IBuilderContext context)
        {
            var container = context.NewBuildUp<IUnityContainer>();
            var typeToBuild = typeof(T);
            var results = ResolveAll(container, typeToBuild, typeToBuild).OfType<T>().ToList();

            var unique_items = new HashSet<T>(results);

            // Add parts from the MEF
            var containerPolicy = context.Policies.Get<ICompositionContainerPolicy>(null);
            if (containerPolicy != null)
            {
                var parts = ResolveAll(
                    containerPolicy.CompositionHostWithDefaultProviders,
                    typeToBuild,
                    null);

                parts.ForEach((part) => unique_items.Add((T)part));
                
                results.AddRange(parts.Select(part => (T)part));
            }

            return CheckForDuplicates(results);
        }

        private static object CheckForDuplicates<T>(List<T> results)
        {
            var y = results.Distinct(new ProductComparare<T>((o) => o.GetType().FullName));
            
            return y;
        }

        private static IEnumerable<object> ResolveAll(IUnityContainer container, Type type, Type typeWrapper)
        {
            var names = GetRegisteredNames(container, type);
            if (type.GetTypeInfo().IsGenericType)
            {
                names = names.Concat(GetRegisteredNames(container, type.GetGenericTypeDefinition()));
            }

            return names.Distinct()
                        .Select(t => t.Name)
                        .Select(name => container.Resolve(typeWrapper, name));
        }

        private static IEnumerable<ContainerRegistration> GetRegisteredNames(IUnityContainer container, Type type)
        {
            return container.Registrations.Where(t => t.RegisteredType == type);
        }
    }

    public class ProductComparare<T> : IEqualityComparer<T>
    {
        private Func<T, object> _funcDistinct;

        public ProductComparare(Func<T, object> funcDistinct)
        {
            this._funcDistinct = funcDistinct;
        }

        public bool Equals(T x, T y)
        {
            return _funcDistinct(x).Equals(_funcDistinct(y));
        }

        public int GetHashCode(T obj)
        {
            return this._funcDistinct(obj).GetHashCode();
        }
    }
}
