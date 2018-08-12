#if NET45

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
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            this.ModuleInfo = moduleInfo;
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
        }

        /// <summary>
        /// Getsthe module info.
        /// </summary>
        /// <value>The module info.</value>
        public IModuleInfo ModuleInfo { get; private set; }

        /// <summary>
        /// Gets the bytes received.
        /// </summary>
        /// <value>The bytes received.</value>
        public long BytesReceived { get; private set; }

        /// <summary>
        /// Gets the total bytes to receive.
        /// </summary>
        /// <value>The total bytes to receive.</value>
        public long TotalBytesToReceive { get; private set; }


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
#endif