using System;
using System.ComponentModel;

namespace Prism.Modularity
{
    /// <summary>
    /// Provides progress information as a module downloads.
    /// </summary>
    public class ModuleDownloadProgressChangedEventArgs : ProgressChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleDownloadProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        public ModuleDownloadProgressChangedEventArgs(IModuleInfo moduleInfo, long bytesReceived, long totalBytesToReceive)
            : base(CalculateProgressPercentage(bytesReceived, totalBytesToReceive), null)
        {
            ModuleInfo = moduleInfo ?? throw new ArgumentNullException(nameof(moduleInfo));
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
        }

        /// <summary>
        /// Gets the module info.
        /// </summary>
        /// <value>The module info.</value>
        public IModuleInfo ModuleInfo { get; }

        /// <summary>
        /// Gets the bytes received.
        /// </summary>
        /// <value>The bytes received.</value>
        public long BytesReceived { get; }

        /// <summary>
        /// Gets the total bytes to receive.
        /// </summary>
        /// <value>The total bytes to receive.</value>
        public long TotalBytesToReceive { get; }

        private static int CalculateProgressPercentage(long bytesReceived, long totalBytesToReceive)
        {
            if ((bytesReceived == 0L) || (totalBytesToReceive == 0L) || (totalBytesToReceive == -1L))
            {
                return 0;
            }

            return (int)((bytesReceived * 100L) / totalBytesToReceive);
        }
    }
}
