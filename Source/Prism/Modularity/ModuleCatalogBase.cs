using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Prism.Modularity
{
    /// <summary>
    /// The <see cref="ModuleCatalogBase"/> holds information about the modules that can be used by the
    /// application. Each module is described in a <see cref="IModuleInfo"/> class, that records the
    /// name, type and location of the module.
    ///
    /// It also verifies that the <see cref="ModuleCatalogBase"/> is internally valid. That means that
    /// it does not have:
    /// <list>
    ///     <item>Circular dependencies</item>
    ///     <item>Missing dependencies</item>
    ///     <item>
    ///         Invalid dependencies, such as a Module that's loaded at startup that depends on a module
    ///         that might need to be retrieved.
    ///     </item>
    /// </list>
    /// The <see cref="ModuleCatalogBase"/> also serves as a baseclass for more specialized Catalogs .
    /// </summary>
    public class ModuleCatalogBase : IModuleCatalog
    {
        private ModuleCatalogItemCollection _items { get; }
        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="IModuleCatalog"/> class.
        /// </summary>
        public ModuleCatalogBase()
        {
            _items = new ModuleCatalogItemCollection();
            _items.CollectionChanged += ItemsCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IModuleCatalog"/> class while providing an
        /// initial list of <see cref="IModuleInfo"/>s.
        /// </summary>
        /// <param name="modules">The initial list of modules.</param>
        public ModuleCatalogBase(IEnumerable<IModuleInfo> modules)
            : this()
        {
            if (modules == null) throw new ArgumentNullException(nameof(modules));
            foreach (IModuleInfo moduleInfo in modules)
            {
                Items.Add(moduleInfo);
            }
        }

        /// <summary>
        /// Gets the items in the <see cref="IModuleCatalog"/>. This property is mainly used to add <see cref="IModuleInfoGroup"/>s or
        /// <see cref="IModuleInfo"/>s through XAML.
        /// </summary>
        /// <value>The items in the catalog.</value>
        public Collection<IModuleCatalogItem> Items => _items;

        /// <summary>
        /// Gets all the <see cref="IModuleInfo"/> classes that are in the <see cref="IModuleCatalog"/>, regardless
        /// if they are within a <see cref="IModuleInfoGroup"/> or not.
        /// </summary>
        /// <value>The modules.</value>
        public virtual IEnumerable<IModuleInfo> Modules => GrouplessModules.Union(Groups.SelectMany(g => g));

        /// <summary>
        /// Gets the <see cref="IModuleInfoGroup"/>s that have been added to the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <value>The groups.</value>
        public IEnumerable<IModuleInfoGroup> Groups => Items.OfType<IModuleInfoGroup>();

        /// <summary>
        /// Gets or sets a value that remembers whether the <see cref="ModuleCatalogBase"/> has been validated already.
        /// </summary>
        protected bool Validated { get; set; }

        /// <summary>
        /// Returns the list of <see cref="IModuleInfo"/>s that are not contained within any <see cref="IModuleInfoGroup"/>.
        /// </summary>
        /// <value>The groupless modules.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Groupless")]
        protected IEnumerable<IModuleInfo> GrouplessModules => Items.OfType<IModuleInfo>();

        /// <summary>
        /// Loads the catalog if necessary.
        /// </summary>
        public virtual void Load()
        {
            isLoaded = true;
            InnerLoad();
        }

        /// <summary>
        /// Return the list of <see cref="IModuleInfo"/>s that <paramref name="moduleInfo"/> depends on.
        /// </summary>
        /// <remarks>
        /// If  the <see cref="IModuleCatalog"/> was not yet validated, this method will call <see cref="Validate"/>.
        /// </remarks>
        /// <param name="moduleInfo">The <see cref="IModuleInfo"/> to get the </param>
        /// <returns>An enumeration of <see cref="IModuleInfo"/> that <paramref name="moduleInfo"/> depends on.</returns>
        public virtual IEnumerable<IModuleInfo> GetDependentModules(IModuleInfo moduleInfo)
        {
            this.EnsureCatalogValidated();

            return this.GetDependentModulesInner(moduleInfo);
        }

        /// <summary>
        /// Returns a list of <see cref="IModuleInfo"/>s that contain both the <see cref="IModuleInfo"/>s in
        /// <paramref name="modules"/>, but also all the modules they depend on.
        /// </summary>
        /// <param name="modules">The modules to get the dependencies for.</param>
        /// <returns>
        /// A list of <see cref="IModuleInfo"/> that contains both all <see cref="IModuleInfo"/>s in <paramref name="modules"/>
        /// but also all the <see cref="IModuleInfo"/> they depend on.
        /// </returns>
        public virtual IEnumerable<IModuleInfo> CompleteListWithDependencies(IEnumerable<IModuleInfo> modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            EnsureCatalogValidated();

            List<IModuleInfo> completeList = new List<IModuleInfo>();
            List<IModuleInfo> pendingList = modules.ToList();
            while (pendingList.Count > 0)
            {
                var moduleInfo = pendingList[0];

                foreach (var dependency in GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }

                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }

            IEnumerable<IModuleInfo> sortedList = Sort(completeList);
            return sortedList;
        }

        /// <summary>
        /// Validates the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="IModuleCatalog"/> fails.</exception>
        public virtual void Validate()
        {
            ValidateUniqueModules();
            ValidateDependencyGraph();
            ValidateCrossGroupDependencies();
            ValidateDependenciesInitializationMode();

            Validated = true;
        }

        /// <summary>
        /// Adds a <see cref="IModuleInfo"/> to the <see cref="IModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="IModuleInfo"/> to add.</param>
        /// <returns>The <see cref="IModuleCatalog"/> for easily adding multiple modules.</returns>
        public virtual IModuleCatalog AddModule(IModuleInfo moduleInfo)
        {
            Items.Add(moduleInfo);
            return this;
        }

        /// <summary>
        /// Initializes the catalog, which may load and validate the modules.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalogBase"/> fails, because this method calls <see cref="Validate"/>.</exception>
        public virtual void Initialize()
        {
            if (!isLoaded)
            {
                Load();
            }

            Validate();
        }

        /// <summary>
        /// Checks for cyclic dependencies, by calling the dependencysolver.
        /// </summary>
        /// <param name="modules">the.</param>
        /// <returns></returns>
        protected static string[] SolveDependencies(IEnumerable<IModuleInfo> modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            ModuleDependencySolver solver = new ModuleDependencySolver();

            foreach (var data in modules)
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
        /// Ensures that all the dependencies within <paramref name="modules"/> refer to <see cref="IModuleInfo"/>s
        /// within that list.
        /// </summary>
        /// <param name="modules">The modules to validate modules for.</param>
        /// <exception cref="ModularityException">
        /// Throws if a <see cref="IModuleInfo"/> in <paramref name="modules"/> depends on a module that's
        /// not in <paramref name="modules"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="modules"/> is <see langword="null"/>.</exception>
        protected static void ValidateDependencies(IEnumerable<IModuleInfo> modules)
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));

            var moduleNames = modules.Select(m => m.ModuleName).ToList();
            foreach (var moduleInfo in modules)
            {
                if (moduleInfo.DependsOn != null && moduleInfo.DependsOn.Except(moduleNames).Any())
                {
                    throw new ModularityException(
                        moduleInfo.ModuleName,
                        string.Format(CultureInfo.CurrentCulture, Resources.ModuleDependenciesNotMetInGroup, moduleInfo.ModuleName));
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
        /// Sorts a list of <see cref="IModuleInfo"/>s. This method is called by <see cref="CompleteListWithDependencies"/>
        /// to return a sorted list.
        /// </summary>
        /// <param name="modules">The <see cref="IModuleInfo"/>s to sort.</param>
        /// <returns>Sorted list of <see cref="IModuleInfo"/>s</returns>
        protected virtual IEnumerable<IModuleInfo> Sort(IEnumerable<IModuleInfo> modules)
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
            List<string> moduleNames = Modules.Select(m => m.ModuleName).ToList();

            string duplicateModule = moduleNames.FirstOrDefault(
                m => moduleNames.Count(m2 => m2 == m) > 1);

            if (duplicateModule != null)
            {
                throw new DuplicateModuleException(duplicateModule, string.Format(CultureInfo.CurrentCulture, Resources.DuplicatedModule, duplicateModule));
            }
        }

        /// <summary>
        /// Ensures that there are no cyclic dependencies.
        /// </summary>
        protected virtual void ValidateDependencyGraph()
        {
            SolveDependencies(Modules);
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
            ValidateDependencies(GrouplessModules);
            foreach (var group in Groups)
            {
                ValidateDependencies(GrouplessModules.Union(group));
            }
        }

        /// <summary>
        /// Ensures that there are no modules marked to be loaded <see cref="InitializationMode.WhenAvailable"/>
        /// depending on modules loaded <see cref="InitializationMode.OnDemand"/>
        /// </summary>
        protected virtual void ValidateDependenciesInitializationMode()
        {
            var moduleInfo = Modules.FirstOrDefault(
                m =>
                m.InitializationMode == InitializationMode.WhenAvailable &&
                GetDependentModulesInner(m)
                    .Any(dependency => dependency.InitializationMode == InitializationMode.OnDemand));

            if (moduleInfo != null)
            {
                throw new ModularityException(
                    moduleInfo.ModuleName,
                    string.Format(CultureInfo.CurrentCulture, Resources.StartupModuleDependsOnAnOnDemandModule, moduleInfo.ModuleName));
            }
        }

        /// <summary>
        /// Returns the <see cref="IModuleInfo"/> on which the received module dependens on.
        /// </summary>
        /// <param name="moduleInfo">Module whose dependant modules are requested.</param>
        /// <returns>Collection of <see cref="IModuleInfo"/> dependants of <paramref name="moduleInfo"/>.</returns>
        protected virtual IEnumerable<IModuleInfo> GetDependentModulesInner(IModuleInfo moduleInfo)
        {
            return Modules.Where(dependantModule => moduleInfo.DependsOn.Contains(dependantModule.ModuleName));
        }

        /// <summary>
        /// Ensures that the catalog is validated.
        /// </summary>
        protected virtual void EnsureCatalogValidated()
        {
            if (!Validated)
            {
                Validate();
            }
        }

        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Validated)
            {
                EnsureCatalogValidated();
            }
        }

        private class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            protected override void InsertItem(int index, IModuleCatalogItem item)
            {
                base.InsertItem(index, item);

                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }

            protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
            {
                CollectionChanged?.Invoke(this, eventArgs);
            }
        }
    }
}
