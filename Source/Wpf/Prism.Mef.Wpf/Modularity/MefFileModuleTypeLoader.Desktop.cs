using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Prism.Modularity;

namespace Prism.Mef.Modularity
{
    /// <summary>
    /// Loads modules from an arbitrary location on the filesystem. This typeloader is only called if
    /// <see cref="ModuleInfo"/> classes have a Ref parameter that starts with "file://".
    /// This class is only used on the Desktop version of the Prism Library when used with Managed Extensibility Framework.
    /// </summary>
    [Export]
    public class MefFileModuleTypeLoader : IModuleTypeLoader
    {
        private const string RefFilePrefix = "file://";
        private readonly HashSet<Uri> downloadedUris = new HashSet<Uri>();

        // disable the warning that the field is never assigned to, and will always have its default value null
        // as it is imported by MEF
#pragma warning disable 0649
        [Import(AllowRecomposition = false)]
        private AggregateCatalog aggregateCatalog;
#pragma warning restore 0649

        /// <summary>
        /// Initializes a new instance of the MefFileModuleTypeLoader class.
        /// This instance is used to load requested module types.
        /// </summary>
        public MefFileModuleTypeLoader()
        {
        }

        #region IModuleTypeLoader Members

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public virtual event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public virtual event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        /// <summary>
        /// Evaluates the <see cref="ModuleInfo.Ref"/> property to see if the current typeloader will be able to retrieve the <paramref name="moduleInfo"/>.
        /// Returns true if the <see cref="ModuleInfo.Ref"/> property starts with "file://", because this indicates that the file
        /// is a local file.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        /// <returns>
        ///     <see langword="true"/> if the current typeloader is able to retrieve the module, otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(RefFilePrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Retrieves the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        public virtual void LoadModuleType(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            try
            {
                Uri uri = new Uri(moduleInfo.Ref, UriKind.RelativeOrAbsolute);

                // If this module has already been downloaded, I fire the completed event.
                if (this.IsSuccessfullyDownloaded(uri))
                {
                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
                else
                {
                    string path;

                    if (moduleInfo.Ref.StartsWith(RefFilePrefix + "/", StringComparison.Ordinal))
                    {
                        path = moduleInfo.Ref.Substring(RefFilePrefix.Length + 1);
                    }
                    else
                    {
                        path = moduleInfo.Ref.Substring(RefFilePrefix.Length);
                    }

                    long fileSize = -1L;
                    if (File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        fileSize = fileInfo.Length;
                    }

                    // Although this isn't asynchronous, nor expected to take very long, I raise progress changed for consistency.
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, 0, fileSize);

                    this.aggregateCatalog.Catalogs.Add(new AssemblyCatalog(path));

                    // Although this isn't asynchronous, nor expected to take very long, I raise progress changed for consistency.
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, fileSize, fileSize);

                    // I remember the downloaded URI.
                    this.RecordDownloadSuccess(uri);

                    this.RaiseLoadModuleCompleted(moduleInfo, null);
                }
            }
            catch (Exception ex)
            {
                this.RaiseLoadModuleCompleted(moduleInfo, ex);
            }
        }

        #endregion

        private void RaiseModuleDownloadProgressChanged(
            ModuleInfo moduleInfo,
            long bytesReceived,
            long totalBytesToReceive)
        {
            this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(
                                                        moduleInfo, bytesReceived, totalBytesToReceive));
        }

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            if (this.ModuleDownloadProgressChanged != null)
            {
                this.ModuleDownloadProgressChanged(this, e);
            }
        }

        private void RaiseLoadModuleCompleted(ModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            if (this.LoadModuleCompleted != null)
            {
                this.LoadModuleCompleted(this, e);
            }
        }

        private bool IsSuccessfullyDownloaded(Uri uri)
        {
            lock (this.downloadedUris)
            {
                return this.downloadedUris.Contains(uri);
            }
        }

        private void RecordDownloadSuccess(Uri uri)
        {
            lock (this.downloadedUris)
            {
                this.downloadedUris.Add(uri);
            }
        }
    }
}