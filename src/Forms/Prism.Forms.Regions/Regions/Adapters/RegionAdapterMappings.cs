using System;
using System.Collections.Generic;
using System.Globalization;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Regions.Adapters
{
    /// <summary>
    /// This class maps <see cref="Type"/> with <see cref="IRegionAdapter"/>.
    /// </summary>
    public class RegionAdapterMappings
    {
        private readonly Dictionary<Type, IRegionAdapter> mappings = new Dictionary<Type, IRegionAdapter>();

        /// <summary>
        /// Registers the mapping between a type and an adapter.
        /// </summary>
        /// <typeparam name="TControl">The type of the control</typeparam>
        /// <typeparam name="TAdapter">The type of the IRegionAdapter to use with the TControl</typeparam>
        /// <exception cref="InvalidOperationException">Throws <see cref="InvalidOperationException"/> when a mapping has already been defined for a specified control type.</exception>
        public void RegisterMapping<TControl, TAdapter>() where TAdapter : IRegionAdapter
        {
            var controlType = typeof(TControl);

            if (mappings.ContainsKey(controlType))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                  Resources.MappingExistsException, controlType.Name));

            var adapter = ContainerLocator.Container.Resolve<TAdapter>();

            mappings.Add(controlType, adapter);
        }

        /// <summary>
        /// Removes an existing Registration if one exists and registers the new mapping between a type and an adapter.
        /// </summary>
        /// <typeparam name="TControl">The type of the control</typeparam>
        /// <typeparam name="TAdapter">The type of the IRegionAdapter to use with the TControl</typeparam>
        public void RegisterOrReplaceMapping<TControl, TAdapter>() where TAdapter : IRegionAdapter
        {
            var controlType = typeof(TControl);
            var adapter = ContainerLocator.Container.Resolve<TAdapter>();

            if (mappings.ContainsKey(controlType))
                mappings.Remove(controlType);

            mappings.Add(controlType, adapter);
        }

        internal void RegisterDefaultMapping<TControl, TAdapter>() where TAdapter : IRegionAdapter
        {
            var controlType = typeof(TControl);

            if (mappings.ContainsKey(controlType))
                return;

            var adapter = ContainerLocator.Container.Resolve<TAdapter>();

            mappings.Add(controlType, adapter);
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
        public IRegionAdapter GetMapping(Type controlType)
        {
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
        }

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
