using System;
using System.Collections.Generic;
using System.IO;

namespace Prism.Modularity
{
    /// <summary>
    /// Loads modules from an arbitrary location on the filesystem. This typeloader is only called if
    /// <see cref="ModuleInfo"/> classes have a Ref parameter that starts with "file://".
    /// This class is only used on the Desktop version of the Prism Library.
    /// </summary>
    public class FileModuleTypeLoader : IModuleTypeLoader, IDisposable
    {
        private const string RefFilePrefix = "file://";

        private readonly IAssemblyResolver assemblyResolver;
        private HashSet<Uri> downloadedUris = new HashSet<Uri>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModuleTypeLoader"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is disposed of in the Dispose method.")]
        public FileModuleTypeLoader()
            : this(new AssemblyResolver())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileModuleTypeLoader"/> class.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        public FileModuleTypeLoader(IAssemblyResolver assemblyResolver)
        {
            this.assemblyResolver = assemblyResolver;
        }

        /// <summary>
        /// Raised repeatedly to provide progress as modules are loaded in the background.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        private void RaiseModuleDownloadProgressChanged(IModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
        {
            this.RaiseModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, bytesReceived, totalBytesToReceive));
        }

        private void RaiseModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            ModuleDownloadProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raised when a module is loaded or fails to load.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private void RaiseLoadModuleCompleted(IModuleInfo moduleInfo, Exception error)
        {
            this.RaiseLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        private void RaiseLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            this.LoadModuleCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// Evaluates the <see cref="IModuleInfo.Ref"/> property to see if the current typeloader will be able to retrieve the <paramref name="moduleInfo"/>.
        /// Returns true if the <see cref="IModuleInfo.Ref"/> property starts with "file://", because this indicates that the file
        /// is a local file.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        /// <returns>
        /// 	<see langword="true"/> if the current typeloader is able to retrieve the module, otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="moduleInfo"/> is null.</exception>
        public bool CanLoadModuleType(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new ArgumentNullException(nameof(moduleInfo));
            }

            return moduleInfo.Ref != null && moduleInfo.Ref.StartsWith(RefFilePrefix, StringComparison.Ordinal);
        }


        /// <summary>
        /// Retrieves the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">Module that should have it's type loaded.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is rethrown as part of a completion event")]
        public void LoadModuleType(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                throw new ArgumentNullException(nameof(moduleInfo));
            }

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
                    string path = uri.LocalPath;

                    long fileSize = -1L;
                    if (File.Exists(path))
                    {
                        FileInfo fileInfo = new FileInfo(path);
                        fileSize = fileInfo.Length;
                    }

                    // Although this isn't asynchronous, nor expected to take very long, I raise progress changed for consistency.
                    this.RaiseModuleDownloadProgressChanged(moduleInfo, 0, fileSize);

                    this.assemblyResolver.LoadAssemblyFrom(moduleInfo.Ref);

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

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>Calls <see cref="Dispose(bool)"/></remarks>.
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the associated <see cref="AssemblyResolver"/>.
        /// </summary>
        /// <param name="disposing">When <see langword="true"/>, it is being called from the Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.assemblyResolver is IDisposable disposableResolver)
            {
                disposableResolver.Dispose();
            }
        }

        #endregion
    }
}
