namespace Prism.Composition.Windows.Policies
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Build plan which enables true support for <see cref="Lazy{T}"/>.
    /// </summary>
    public class LazyResolveBuildPlanPolicy : IBuildPlanPolicy
    {
        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        public void BuildUp(IBuilderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.Existing == null)
            {
                var currentContainer = context.NewBuildUp<IUnityContainer>();

                var typeToBuild = GetTypeToBuild(context.BuildKey.Type);

                var nameToBuild = context.BuildKey.Name;

                context.Existing = IsResolvingIEnumerable(typeToBuild) ?
                    CreateResolveAllResolver(currentContainer, typeToBuild) :
                    CreateResolver(currentContainer, typeToBuild, nameToBuild);

                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> to build from the <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance</param>
        /// <returns>The <see cref="Type"/> returned</returns>
        private static Type GetTypeToBuild(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// Is the <see cref="Type"/> instance a <see cref="IEnumerable{T}"/>?
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance</param>
        /// <returns>Returns true if the instance is of type <see cref="IEnumerable{T}"/></returns>
        private static bool IsResolvingIEnumerable(Type type)
        {
            return type.GetTypeInfo().IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        /// <summary>
        /// Creates a Resolver
        /// </summary>
        /// <param name="currentContainer">The <see cref="IUnityContainer"/> that will be used.</param>
        /// <param name="typeToBuild">The <see cref="Type"/> to build.</param>
        /// <param name="nameToBuild">The text associated with the build.</param>
        /// <returns>The created resolver</returns>
        private static object CreateResolver(
            IUnityContainer currentContainer, 
            Type typeToBuild,
            string nameToBuild)
        {
            Type lazyType = typeof(Lazy<>).MakeGenericType(typeToBuild);

            Type trampolineType = typeof(ResolveTrampoline<>).MakeGenericType(typeToBuild);

            Type delegateType = typeof(Func<>).MakeGenericType(typeToBuild);

            MethodInfo resolveMethod = trampolineType.GetMethod("Resolve");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer, nameToBuild);

            object trampolineDelegate = resolveMethod.CreateDelegate(delegateType, trampoline);

            return Activator.CreateInstance(lazyType, trampolineDelegate);
        }

        /// <summary>
        /// Creates a Resolver
        /// </summary>
        /// <param name="currentContainer">The <see cref="IUnityContainer"/> that will be used.</param>
        /// <param name="enumerableType">The <see cref="Type"/> that should be in an <see cref="IEnumerable{T}"/></param>
        /// <returns>The created resolver</returns>
        private static object CreateResolveAllResolver(IUnityContainer currentContainer, Type enumerableType)
        {
            Type typeToBuild = GetTypeToBuild(enumerableType);

            Type lazyType = typeof(Lazy<>).MakeGenericType(enumerableType);

            Type trampolineType = typeof(ResolveAllTrampoline<>).MakeGenericType(typeToBuild);

            Type delegateType = typeof(Func<>).MakeGenericType(enumerableType);

            MethodInfo resolveAllMethod = trampolineType.GetMethod("ResolveAll");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer);

            object trampolineDelegate = resolveAllMethod.CreateDelegate(delegateType, trampoline);

            return Activator.CreateInstance(lazyType, trampolineDelegate);
        }

        /// <summary>
        /// The trampoline class for resolving a single type
        /// </summary>
        /// <typeparam name="T">The type to wrap</typeparam>
        private class ResolveTrampoline<T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ResolveTrampoline{T}"/> class.
            /// </summary>
            /// <param name="container"><see cref="IUnityContainer"/> instance.</param>
            /// <param name="name">The name associated with the build.</param>
            public ResolveTrampoline(IUnityContainer container, string name)
            {
                this.Container = container;

                this.Name = name;
            }

            /// <summary>
            /// Gets or sets the <see cref="IUnityContainer"/>
            /// </summary>
            private IUnityContainer Container { get; set; }

            /// <summary>
            /// Gets or sets the name associated with the build
            /// </summary>
            private string Name { get; set; }

            /// <summary>
            /// Resolves type from <see cref="IUnityContainer"/>.
            /// </summary>
            /// <returns>The type from <see cref="IUnityContainer"/>.</returns>
            public T Resolve()
            {
                return this.Container.Resolve<T>(this.Name);
            }
        }

        /// <summary>
        /// the trampoline class for resolving many types.
        /// </summary>
        /// <typeparam name="T">The type to wrap</typeparam>
        private class ResolveAllTrampoline<T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ResolveAllTrampoline{T}"/> class.
            /// </summary>
            /// <param name="container"><see cref="IUnityContainer"/> instance.</param>
            public ResolveAllTrampoline(IUnityContainer container)
            {
                this.Container = container;
            }

            /// <summary>
            /// Gets or sets the <see cref="IUnityContainer"/>
            /// </summary>
            private IUnityContainer Container { get; set; }

            /// <summary>
            /// Resolves all the types from <see cref="IUnityContainer"/>.
            /// </summary>
            /// <returns>The types from <see cref="IUnityContainer"/>.</returns>
            public IEnumerable<T> ResolveAll()
            {
                return this.Container.ResolveAll<T>();
            }
        }
    }
}