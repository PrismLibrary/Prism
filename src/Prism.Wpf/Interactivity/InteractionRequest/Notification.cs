

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Interactivity.InteractionRequest
{
    /// <summary>
    /// Basic implementation of <see cref="INotification"/>.
    /// </summary>
    public class Notification : INotification
    {
        /// <summary>
        /// Gets or sets the title to use for the notification.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the content of the notification.
        /// </summary>
        public object Content { get; set; }
    }
}
