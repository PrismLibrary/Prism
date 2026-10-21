using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Prism.Properties;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// This class maps <see cref="Type"/> with <see cref="IRegionAdapter"/>.
    /// </summary>
    public class RegionAdapterMappings
    {
        private readonly Dictionary<Type, IRegionAdapter> mappings = new Dictionary<Type, IRegionAdapter>();
#if WINUI && !UNO_WINUI
        private readonly Dictionary<Type, Type> adapterTypes = new Dictionary<Type, Type>();
#endif

        /// <summary>
        /// Registers the mapping between a type and an adapter.
        /// </summary>
        /// <param name="controlType">The type of the control.</param>
        /// <param name="adapter">The adapter to use with the <paramref name="controlType"/> type.</param>
        /// <exception cref="ArgumentNullException">When any of <paramref name="controlType"/> or <paramref name="adapter"/> are <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">If a mapping for <paramref name="controlType"/> already exists.</exception>
        public void RegisterMapping(Type controlType, IRegionAdapter adapter)
        {
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));

            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            if (mappings.ContainsKey(controlType)
#if WINUI && !UNO_WINUI
                || adapterTypes.ContainsKey(controlType)
#endif
                )
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                  Resources.MappingExistsException, controlType.Name));

            mappings.Add(controlType, adapter);
        }

        /// <summary>
        /// Registers the mapping between a type and an adapter.
        /// </summary>
        /// <typeparam name="TControl">The type of the control</typeparam>
        public void RegisterMapping<TControl>(IRegionAdapter adapter)
        {
            RegisterMapping(typeof(TControl), adapter);
        }

        /// <summary>
        /// Registers the mapping between a type and an adapter.
        /// </summary>
        /// <typeparam name="TControl">The type of the control</typeparam>
        /// <typeparam name="TAdapter">The type of the IRegionAdapter to use with the TControl</typeparam>
        public void RegisterMapping<TControl, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TAdapter>() where TAdapter : IRegionAdapter
        {
#if WINUI && !UNO_WINUI
            var controlType = typeof(TControl);
            if (mappings.ContainsKey(controlType) || adapterTypes.ContainsKey(controlType))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.MappingExistsException, controlType.Name));

            adapterTypes.Add(controlType, typeof(TAdapter));
#else
            RegisterMapping(typeof(TControl), ContainerLocator.Container.Resolve<TAdapter>());
#endif
        }

        /// <summary>
        /// Returns the adapter associated with the type provided.
        /// </summary>
        /// <param name="controlType">The type to obtain the <see cref="IRegionAdapter"/> mapped.</param>
        /// <returns>The <see cref="IRegionAdapter"/> mapped to the <paramref name="controlType"/>.</returns>
        /// <remarks>This class will look for a registered type for <paramref name="controlType"/> and if there is not any,
        /// it will look for a registered type for any of its ancestors in the class hierarchy.
        /// If there is no registered type for <paramref name="controlType"/> or any of its ancestors,
        /// an exception will be thrown.</remarks>
        /// <exception cref="KeyNotFoundException">When there is no registered type for <paramref name="controlType"/> or any of its ancestors.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "controlType")]
        public IRegionAdapter GetMapping(Type controlType)
        {
#if WINUI && !UNO_WINUI
            return GetMapping(controlType, null);
#else
            Type currentType = controlType;

            while (currentType != null)
            {
                if (mappings.ContainsKey(currentType))
                {
                    return mappings[currentType];
                }

                currentType = currentType.BaseType;
            }

            throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, Resources.NoRegionAdapterException, controlType));
#endif
        }

#if WINUI && !UNO_WINUI
        internal IRegionAdapter GetMapping(Type controlType, IContainerProvider provider)
        {
            for (var currentType = controlType; currentType is not null; currentType = currentType.BaseType)
            {
                if (mappings.TryGetValue(currentType, out var adapter))
                    return adapter;

                if (adapterTypes.TryGetValue(currentType, out var adapterType))
                {
                    if (provider is null)
                        throw new InvalidOperationException("Native WinUI region adapters require an originating window container provider.");

                    return (IRegionAdapter)provider.Resolve(adapterType, (typeof(IContainerProvider), provider));
                }
            }

            throw new KeyNotFoundException(string.Format(CultureInfo.CurrentCulture, Resources.NoRegionAdapterException, controlType));
        }
#endif

        /// <summary>
        /// Returns the adapter associated with the type provided.
        /// </summary>
        /// <typeparam name="T">The control type used to obtain the <see cref="IRegionAdapter"/> mapped.</typeparam>
        /// <returns>The <see cref="IRegionAdapter"/> mapped to the <typeparamref name="T"/>.</returns>
        /// <remarks>This class will look for a registered type for <typeparamref name="T"/> and if there is not any,
        /// it will look for a registered type for any of its ancestors in the class hierarchy.
        /// If there is no registered type for <typeparamref name="T"/> or any of its ancestors,
        /// an exception will be thrown.</remarks>
        /// <exception cref="KeyNotFoundException">When there is no registered type for <typeparamref name="T"/> or any of its ancestors.</exception>
        public IRegionAdapter GetMapping<T>()
        {
            return GetMapping(typeof(T));
        }
    }
}
