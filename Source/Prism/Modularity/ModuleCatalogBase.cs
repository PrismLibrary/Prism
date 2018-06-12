using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace Prism.Modularity
{
    public abstract class ModuleCatalogBase : IModuleCatalog
    {
        private readonly ModuleCatalogItemCollection items;
        protected bool isLoaded;

        public ModuleCatalogBase()
        {
            items = new ModuleCatalogItemCollection();
            items.CollectionChanged += ItemsCollectionChanged;
        }

        public ModuleCatalogBase(IEnumerable<ModuleInfo> modules)
            : this()
        {
            if (modules == null) throw new ArgumentNullException(nameof(modules));
            foreach (ModuleInfo moduleInfo in modules)
            {
                Items.Add(moduleInfo);
            }
        }

        /// <summary>
        /// Gets the items in the <see cref="ModuleCatalog"/>. This property is mainly used to add <see cref="ModuleInfoGroup"/>s or
        /// <see cref="ModuleInfo"/>s through XAML.
        /// </summary>
        /// <value>The items in the catalog.</value>
        public Collection<IModuleCatalogItem> Items => items;

        /// <summary>
        /// Gets all the <see cref="ModuleInfo"/> classes that are in the <see cref="ModuleCatalog"/>, regardless
        /// if they are within a <see cref="ModuleInfoGroup"/> or not.
        /// </summary>
        /// <value>The modules.</value>
        public virtual IEnumerable<ModuleInfo> Modules => GrouplessModules.Union(Groups.SelectMany(g => g));

        /// <summary>
        /// Gets the <see cref="ModuleInfoGroup"/>s that have been added to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <value>The groups.</value>
        public IEnumerable<ModuleInfoGroup> Groups => Items.OfType<ModuleInfoGroup>();

        /// <summary>
        /// Gets or sets a value that remembers whether the <see cref="ModuleCatalog"/> has been validated already.
        /// </summary>
        protected bool Validated { get; set; }

        /// <summary>
        /// Returns the list of <see cref="ModuleInfo"/>s that are not contained within any <see cref="ModuleInfoGroup"/>.
        /// </summary>
        /// <value>The groupless modules.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Groupless")]
        protected IEnumerable<ModuleInfo> GrouplessModules => Items.OfType<ModuleInfo>();

        /// <summary>
        /// Adds a <see cref="ModuleInfo"/> to the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <param name="moduleInfo">The <see cref="ModuleInfo"/> to add.</param>
        /// <returns>The <see cref="ModuleCatalog"/> for easily adding multiple modules.</returns>
        public virtual IModuleCatalog AddModule(ModuleInfo moduleInfo)
        {
            Items.Add(moduleInfo);
            return this;
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

            EnsureCatalogValidated();

            List<ModuleInfo> completeList = new List<ModuleInfo>();
            List<ModuleInfo> pendingList = modules.ToList();
            while (pendingList.Count > 0)
            {
                ModuleInfo moduleInfo = pendingList[0];

                foreach (ModuleInfo dependency in GetDependentModules(moduleInfo))
                {
                    if (!completeList.Contains(dependency) && !pendingList.Contains(dependency))
                    {
                        pendingList.Add(dependency);
                    }
                }

                pendingList.RemoveAt(0);
                completeList.Add(moduleInfo);
            }

            IEnumerable<ModuleInfo> sortedList = Sort(completeList);
            return sortedList;
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
            EnsureCatalogValidated();

            return GetDependentModulesInner(moduleInfo);
        }

        public virtual void Initialize()
        {
            if (!isLoaded)
            {
                Load();
            }

            Validate();
        }

        /// <summary>
        /// Loads the catalog if necessary.
        /// </summary>
        public void Load()
        {
            isLoaded = true;
            InnerLoad();
        }

        /// <summary>
        /// Does the actual work of loading the catalog. The base implementation does nothing.
        /// </summary>
        protected virtual void InnerLoad()
        {
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
                        string.Format(CultureInfo.CurrentCulture, Resources.ModuleDependenciesNotMetInGroup, moduleInfo.ModuleName));
                }
            }
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
        /// Validates the <see cref="ModuleCatalog"/>.
        /// </summary>
        /// <exception cref="ModularityException">When validation of the <see cref="ModuleCatalog"/> fails.</exception>
        public virtual void Validate()
        {
            ValidateUniqueModules();
            ValidateDependencyGraph();
            ValidateCrossGroupDependencies();
            ValidateDependenciesInitializationMode();

            Validated = true;
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
            foreach (ModuleInfoGroup group in Groups)
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
            ModuleInfo moduleInfo = Modules.FirstOrDefault(
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
        /// Returns the <see cref="ModuleInfo"/> on which the received module dependens on.
        /// </summary>
        /// <param name="moduleInfo">Module whose dependant modules are requested.</param>
        /// <returns>Collection of <see cref="ModuleInfo"/> dependants of <paramref name="moduleInfo"/>.</returns>
        protected virtual IEnumerable<ModuleInfo> GetDependentModulesInner(ModuleInfo moduleInfo)
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

        protected class ModuleCatalogItemCollection : Collection<IModuleCatalogItem>, INotifyCollectionChanged
        {
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            protected override void InsertItem(int index, IModuleCatalogItem item)
            {
                base.InsertItem(index, item);

                OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }

            protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs eventArgs) => 
                CollectionChanged?.Invoke(this, eventArgs);
        }
    }
}
