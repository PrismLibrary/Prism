

using System;
using System.Collections.Generic;
using System.Configuration;

namespace Prism.Modularity
{
    /// <summary>
    /// A collection of <see cref="ModuleConfigurationElement"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class ModuleConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ModuleConfigurationElementCollection"/>.
        /// </summary>
        public ModuleConfigurationElementCollection()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ModuleConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="modules">The initial set of <see cref="ModuleConfigurationElement"/>.</param>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="modules"/> is <see langword="null"/>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ModuleConfigurationElementCollection(ModuleConfigurationElement[] modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            foreach (ModuleConfigurationElement module in modules)
            {
                BaseAdd(module);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an exception should be raised if a duplicate element is found.
        /// This property will always return true.
        /// </summary>
        /// <value>A <see cref="bool"/> value.</value>
        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }

        ///<summary>
        ///Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        ///</summary>
        ///<value>
        ///The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> of this collection.
        ///</value>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        ///<summary>
        ///Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
        ///</summary>
        ///<value>
        ///The name of the collection; otherwise, an empty string.
        ///</value>
        protected override string ElementName
        {
            get { return "module"; }
        }

        /// <summary>
        /// Gets the <see cref="ModuleConfigurationElement"/> located at the specified index in the collection.
        /// </summary>
        /// <param name="index">The index of the element in the collection.</param>
        /// <returns>A <see cref="ModuleConfigurationElement"/>.</returns>
        public ModuleConfigurationElement this[int index]
        {
            get { return (ModuleConfigurationElement)base.BaseGet(index); }
        }

        /// <summary>
        /// Adds a <see cref="ModuleConfigurationElement"/> to the collection.
        /// </summary>
        /// <param name="module">A <see cref="ModuleConfigurationElement"/> instance.</param>
        public void Add(ModuleConfigurationElement module)
        {
            BaseAdd(module);
        }

        /// <summary>
        /// Tests if the collection contains the configuration for the specified module name.
        /// </summary>
        /// <param name="moduleName">The name of the module to search the configuration for.</param>
        /// <returns><see langword="true"/> if a configuration for the module is present; otherwise <see langword="false"/>.</returns>
        public bool Contains(string moduleName)
        {
            return base.BaseGet(moduleName) != null;
        }

        /// <summary>
        /// Searches the collection for all the <see cref="ModuleConfigurationElement"/> that match the specified predicate.
        /// </summary>
        /// <param name="match">A <see cref="Predicate{T}"/> that implements the match test.</param>
        /// <returns>A <see cref="List{T}"/> with the successful matches.</returns>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="match"/> is null.</exception>
        public IList<ModuleConfigurationElement> FindAll(Predicate<ModuleConfigurationElement> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            IList<ModuleConfigurationElement> found = new List<ModuleConfigurationElement>();
            foreach (ModuleConfigurationElement moduleElement in this)
            {
                if (match(moduleElement))
                {
                    found.Add(moduleElement);
                }
            }
            return found;
        }

        /// <summary>
        /// Creates a new <see cref="ModuleConfigurationElement"/>.
        /// </summary>
        /// <returns>A <see cref="ModuleConfigurationElement"/>.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigurationElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for. </param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigurationElement)element).ModuleName;
        }
    }
}
