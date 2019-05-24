#if NETCOREAPP3_0

using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Modularity
{
    public class DirectoryModuleCatalog : ModuleCatalog
    {
        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// Drives the main logic of building the child domain and searching for the assemblies.
        /// </summary>
        protected override void InnerLoad()
        {
            if (string.IsNullOrEmpty(this.ModulePath))
                throw new InvalidOperationException(Resources.ModulePathCannotBeNullOrEmpty);

            if (!Directory.Exists(this.ModulePath))
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.DirectoryNotFound, this.ModulePath));

            AppDomain appDomain = AppDomain.CurrentDomain;

            try
            {
                List<string> loadedAssemblies = new List<string>();

                var assemblies = from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 where !(assembly is System.Reflection.Emit.AssemblyBuilder) 
                                        && assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder"
                                        && !String.IsNullOrEmpty(assembly.Location)
                                 select assembly.Location;

                loadedAssemblies.AddRange(assemblies);

                //Type loaderType = typeof(InnerModuleInfoLoader);

                //if (loaderType.Assembly != null)
                //{
                //    var loader = (InnerModuleInfoLoader)appDomain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
                //    loader.LoadAssemblies(loadedAssemblies);
                //    this.Items.AddRange(loader.GetModuleInfos(this.ModulePath));
                //}
            }
            catch
            {
                //do nothing
            }
        }
    }
}
#endif
