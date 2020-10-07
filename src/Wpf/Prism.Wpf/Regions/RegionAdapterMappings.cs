using System;
using System.Collections.Generic;
using System.Globalization;
using Prism.Ioc;
using Prism.Properties;

namespace Prism.Regions
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

            if (mappings.ContainsKey(controlType))
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
        public void RegisterMapping<TControl, TAdapter>() where TAdapter : IRegionAdapter
        {
            RegisterMapping(typeof(TControl), ContainerLocator.Container.Resolve<TAdapter>());
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
