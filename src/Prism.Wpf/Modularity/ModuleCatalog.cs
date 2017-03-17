

using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Markup;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalog"/> holds information about the modules that can be used by the
    /// application. Each module is described in a <see cref="ModuleInfo"/> class, that records the
    /// name, type and location of the module.
    ///
    /// It also verifies that the <see cref="ModuleCatalog"/> is internally valid. That means that
    /// it does not have:
    /// <list>
    ///     <item>Circular dependencies</item>
    ///     <item>Missing dependencies</item>
    ///     <item>
    ///         Invalid dependencies, such as a Module that's loaded at startup that depends on a module
    ///         that might need to be retrieved.
    ///     </item>
    /// </list>
    /// The <see cref="ModuleCatalog"/> also serves as a baseclass for more specialized Catalogs .
    /// </summary>
    [ContentProperty("Items")]
    public class ModuleCatalog : IModuleCatalog
    {
        private readonly ModuleCatalogItemCollection items;
        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalog()
        {
            this.items = new ModuleCatalogItemCollection();
            this.items.CollectionChanged += this.ItemsCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleCatalog"/> class while providing an
        /// initial list of <see cref="ModuleInfo"/>s.
        /// </summary>
        /// <param name="modules">The initial list of modules.</param>
        public ModuleCatalog(IEnumerable<ModuleInfo> modules)
            : this()
        {
            if (modules == null) throw new ArgumentNullException(nameof(modules));
            foreach (ModuleInfo moduleInfo in modules)
            {
                this.Items.Add(moduleInfo);
            }
        }

        /// <summary>
        /// Gets the items in the <see cref="ModuleCatalog"/>. This property is mainly used to add <see cref="ModuleInfoGroup"/>s or
        /// <see cref="ModuleInfo"/>s through XAML.
        /// </summary>
        /// <value>The items in the catalog.</value>
        public Collection<IModuleCatalogItem> Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Gets all the <see cref="ModuleInfo"/> classes that are in the <see cref="ModuleCatalog"/>, regardless
        /// if they are within a <see cref="ModuleInfoGroup"/> or not.
        /// </summary>
        /// <value>The modules.</value>
        public virtual IEnumerable<ModuleInfo> Modules
        {
            get
            {
                return this.GrouplessModules.Union(this.Groups.SelectMany(g => g));
            }
        }

        /// <summary>
        /// Gets the <see cref="ModuleInfoGroup"/>s that have been added to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <value>The groups.</value>
        public IEnumerable<ModuleInfoGroup> Groups
        {
            get
            {
                return this.Items.OfType<ModuleInfoGroup>();
            }
        }

        /// <summary>
        /// Gets or sets a value that remembers whether the <see cref="ModuleCatalog"/> has been validated already.
        /// </summary>
        protected bool Validated { get; set; }

        /// <summary>
        /// Returns the list of <see cref="ModuleInfo"/>s that are not contained within any <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <value>The groupless modules.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Groupless")]
        protected IEnumerable<ModuleInfo> GrouplessModules
        {
            get
            {
                return this.Items.OfType<ModuleInfo>();
            }
        }

        /// <summary>
        /// Creates a <see cref="ModuleCatalog"/> from XAML.
        /// </summary>
        /// <param name="xamlStream"><see cref="Stream"/> that contains the XAML declaration of the catalog.</param>
        /// <returns>An instance of <see cref="ModuleCatalog"/> built from the XAML.</returns>
        public static ModuleCatalog CreateFromXaml(Stream xamlStream)
        {
            if (xamlStream == null)
            {
                throw new ArgumentNullException(nameof(xamlStream));
            }

            return XamlReader.Load(xamlStream) as ModuleCatalog;
        }

        /// <summary>
        /// Creates a <see cref="ModuleCatalog"/> from a XAML included as an Application Resource.
        /// </summary>
        /// <param name="builderResourceUri">Relative <see cref="Uri"/> that identifies the XAML included as an Application Resource.</param>
        /// <returns>An instance of <see cref="ModuleCatalog"/> build from the XAML.</returns>
        public static ModuleCatalog CreateFromXaml(Uri builderResourceUri)
        {
            var streamInfo = System.Windows.Application.GetResourceStream(builderResourceUri);

            if ((streamInfo != null) && (streamInfo.Stream != null))
            {
                return CreateFromXaml(streamInfo.Stream);
            }

            return null;
        }

        /// <summary>
        /// Loads the catalog if necessary.
        /// </summary>
        public void Load()
        {
            this.isLoaded = true;
            this.InnerLoad();
        }

        /// <summary>
        /// Return the list of <see cref="ModuleInfo"/>s that <paramref name="moduleInfo"/> depends on.
        /// </summary>
        /// <remarks>
        /// If  the <see cref="ModuleCatalog"/> was not yet validated, this method will call <see cref="Validate"/>.
        /// </remarks>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to get the </param>
        /// <returns>An enumeration of <see cref="ModuleInfo"/> that <paramref name="moduleInfo"/> depends on.</returns>
        public virtual IEnumerable<ModuleInfo> GetDependentModules(ModuleInfo moduleInfo)
        {
            this.EnsureCatalogValidated();

            return this.GetDependentModulesInner(moduleInfo);
        }

        /// <summary>
        /// Returns a list of <see cref="ModuleInfo"/>s that contain both the <see cref="ModuleInfo"/>s in
        /// <paramref name="modules"/>, but also all the modules they depend on.
        /// </summary>
        /// <param name="modules">The modules to get the dependencies for.</param>
        /// <returns>
        /// A list of <see cref="ModuleInfo"/> that contains both all <see cref="ModuleInfo"/>s in <paramref name="modules"/>
        /// but also all the <see cref="ModuleInfo"/> they depend on.
        /// </returns>
        public virtual IEnumerable<ModuleInfo> CompleteListWithDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            this.EnsureCatalogValidated();

            List<ModuleInfo> completeList = new List<ModuleInfo>();
            List<ModuleInfo> pendingList = modules.ToList();
            while (pendingList.Count > 0)
            {
                ModuleInfo moduleInfo = pendingList[0];

                foreach (ModuleInfo dependency in this.GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }

                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }

            IEnumerable<ModuleInfo> sortedList = this.Sort(completeList);
            return sortedList;
        }

        /// <summary>
        /// Validates the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails.</exception>
        public virtual void Validate()
        {
            this.ValidateUniqueModules();
            this.ValidateDependencyGraph();
            this.ValidateCrossGroupDependencies();
            this.ValidateDependenciesInitializationMode();

            this.Validated = true;
        }

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to add.</param>
        /// <returns>The <see cref="ModuleCatalog"/> for easily adding multiple modules.</returns>
        public virtual void AddModule(ModuleInfo moduleInfo)
        {
            this.Items.Add(moduleInfo);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(Type moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleType, InitializationMode.WhenAvailable, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(Type moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleType == null) throw new ArgumentNullException(nameof(moduleType));
            return this.AddModule(moduleType.Name, moduleType.AssemblyQualifiedName, initializationMode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, InitializationMode.WhenAvailable, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, InitializationMode initializationMode, params string[] dependsOn)
        {
            return this.AddModule(moduleName, moduleType, null, initializationMode, dependsOn);
        }

        /// <summary>
        /// Adds a groupless <see cref="ModuleInfo"/> to the catalog.
        /// </summary>
        /// <param name="moduleName">Name of the module to be added.</param>
        /// <param name="moduleType"><see cref="Type"/> of the module to be added.</param>
        /// <param name="refValue">Reference to the location of the module to be added assembly.</param>
        /// <param name="initializationMode">Stage on which the module to be added will be initialized.</param>
        /// <param name="dependsOn">Collection of module names (<see cref="ModuleInfo.ModuleName"/>) of the modules on which the module to be added logically depends on.</param>
        /// <returns>The same <see cref="ModuleCatalog"/> instance with the added module.</returns>
        public ModuleCatalog AddModule(string moduleName, string moduleType, string refValue, InitializationMode initializationMode, params string[] dependsOn)
        {
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));

            if (moduleType == null)
                throw new ArgumentNullException(nameof(moduleType));

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, moduleType);
            moduleInfo.DependsOn.AddRange(dependsOn);
            moduleInfo.InitializationMode = initializationMode;
            moduleInfo.Ref = refValue;
            this.Items.Add(moduleInfo);
            return this;
        }

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails, because this method calls <see cref="Validate"/>.</exception>
        public virtual void Initialize()
        {
            if (!this.isLoaded)
            {
                this.Load();
            }

            this.Validate();
        }

        /// <summary>
        /// Creates and adds a <see cref="ModuleInfoGroup"/> to the catalog.
        /// </summary>
        /// <param name="initializationMode">Stage on which the module group to be added will be initialized.</param>
        /// <param name="refValue">Reference to the location of the module group to be added.</param>
        /// <param name="moduleInfos">Collection of <see cref="ModuleInfo"/> included in the group.</param>
        /// <returns><see cref="ModuleCatalog"/> with the added module group.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public virtual ModuleCatalog AddGroup(InitializationMode initializationMode, string refValue, params ModuleInfo[] moduleInfos)
        {
            if (moduleInfos == null)
                throw new ArgumentNullException(nameof(moduleInfos));

            ModuleInfoGroup newGroup = new ModuleInfoGroup();
            newGroup.InitializationMode = initializationMode;
            newGroup.Ref = refValue;

            foreach (ModuleInfo info in moduleInfos)
            {
                newGroup.Add(info);
            }

            this.items.Add(newGroup);

            return this;
        }

        /// <summary>
        /// Checks for cyclic dependencies, by calling the dependencysolver.
        /// </summary>
        /// <param name="modules">the.</param>
        /// <returns></returns>
        protected static string[] SolveDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            ModuleDependencySolver solver = new ModuleDependencySolver();

            foreach (ModuleInfo data in modules)
            {
                solver.AddModule(data.ModuleName);

                if (data.DependsOn != null)
                {
                    foreach (string dependency in data.DependsOn)
                    {
                        solver.AddDependency(data.ModuleName, dependency);
                    }
                }
            }

            if (solver.ModuleCount > 0)
            {
                return solver.Solve();
            }

            return new string[0];
        }

        /// <summary>
        /// Ensures that all the dependencies within <paramref name="modules"/> refer to <see cref="ModuleInfo"/>s
        /// within that list.
        /// </summary>
        /// <param name="modules">The modules to validate modules for.</param>
        /// <exception cref="ModularityException">
        /// Throws if a <see cref="ModuleInfo"/> in <paramref name="modules"/> depends on a module that's
        /// not in <paramref name="modules"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="modules"/> is <see langword="null"/>.</exception>
        protected static void ValidateDependencies(IEnumerable<ModuleInfo> modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            var moduleNames = modules.Select(m => m.ModuleName).ToList();
            foreach (ModuleInfo moduleInfo in modules)
            {
                if (moduleInfo.DependsOn != null && moduleInfo.DependsOn.Except(moduleNames).Any())
                {
                    throw new ModularityException(
                        moduleInfo.ModuleName,
                        String.Format(CultureInfo.CurrentCulture, Resources.ModuleDependenciesNotMetInGroup, moduleInfo.ModuleName));
                }
            }
        }

        /// <summary>
        /// Does the actual work of loading the catalog.  The base implementation does nothing.
        /// </summary>
        protected virtual void InnerLoad()
        {
        }

        /// <summary>
        /// Sorts a list of <see cref="ModuleInfo"/>s. This method is called by <see cref="CompleteListWithDependencies"/>
        /// to return a sorted list.
        /// </summary>
        /// <param name="modules">The <see cref="ModuleInfo"/>s to sort.</param>
        /// <returns>Sorted list of <see cref="ModuleInfo"/>s</returns>
        protected virtual IEnumerable<ModuleInfo> Sort(IEnumerable<ModuleInfo> modules)
        {
            foreach (string moduleName in SolveDependencies(modules))
            {
                yield return modules.First(m => m.ModuleName == moduleName);
            }
        }

        /// <summary>
        /// Makes sure all modules have an Unique name.
        /// </summary>
        /// <exception cref="DuplicateModuleException">
        /// Thrown if the names of one or more modules are not unique.
        /// </exception>
        protected virtual void ValidateUniqueModules()
        {
            List<string> moduleNames = this.Modules.Select(m => m.ModuleName).ToList();

            string duplicateModule = moduleNames.FirstOrDefault(
                m => moduleNames.Count(m2 => m2 == m) > 1);

            if (duplicateModule != null)
            {
                throw new DuplicateModuleException(duplicateModule, String.Format(CultureInfo.CurrentCulture, Resources.DuplicatedModule, duplicateModule));
            }
        }

        /// <summary>
        /// Ensures that there are no cyclic dependencies.
        /// </summary>
        protected virtual void ValidateDependencyGraph()
        {
            SolveDependencies(this.Modules);
        }

        /// <summary>
        /// Ensures that there are no dependencies between modules on different groups.
        /// </summary>
        /// <remarks>
        /// A groupless module can only depend on other groupless modules.
        /// A module within a group can depend on other modules within the same group and/or on groupless modules.
        /// </remarks>
        protected virtual void ValidateCrossGroupDependencies()
        {
            ValidateDependencies(this.GrouplessModules);
            foreach (ModuleInfoGroup group in this.Groups)
            {
                ValidateDependencies(this.GrouplessModules.Union(group));
            }
        }

        /// <summary>
        /// Ensures that there are no modules marked to be loaded <see cref="InitializationMode.WhenAvailable"/>
        /// depending on modules loaded <see cref="InitializationMode.OnDemand"/>
        /// </summary>
        protected virtual void ValidateDependenciesInitializationMode()
        {
            ModuleInfo moduleInfo = this.Modules.FirstOrDefault(
                m =>
                m.InitializationMode == InitializationMode.WhenAvailable &&
                this.GetDependentModulesInner(m)
                    .Any(dependency => dependency.InitializationMode == InitializationMode.OnDemand));

            if (moduleInfo != null)
            {
                throw new ModularityException(
                    moduleInfo.ModuleName,
                    String.Format(CultureInfo.CurrentCulture, Resources.StartupModuleDependsOnAnOnDemandModule, moduleInfo.ModuleName));
            }
        }

        /// <summary>
        /// Returns the <see cref="ModuleInfo"/> on which the received module dependens on.
        /// </summary>
        /// <param name="moduleInfo">Module whose dependant modules are requested.</param>
        /// <returns>Collection of <see cref="ModuleInfo"/> dependants of <paramref name="moduleInfo"/>.</returns>
        protected virtual IEnumerable<ModuleInfo> GetDependentModulesInner(ModuleInfo moduleInfo)
        {
            return this.Modules.Where(dependantModule => moduleInfo.DependsOn.Contains(dependantModule.ModuleName));
        }

        /// <summary>
        /// Ensures that the catalog is validated.
        /// </summary>
        protected virtual void EnsureCatalogValidated()
        {
            if (!this.Validated)
            {
                this.Validate();
            }
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Validated)
            {
                this.EnsureCatalogValidated();
            }
        }

        private class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            protected override void InsertItem(int index, IModuleCatalogItem item)
            {
                base.InsertItem(index, item);

                this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }

            protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
            {
                if (this.CollectionChanged != null)
                {
                    this.CollectionChanged(this, eventArgs);
                }
            }
        }
    }
}
