using Xamarin.Forms;

namespace Prism.Common
{
    /// <summary>
    /// Interface to signify that a class must have knowledge of a specific <see cref="Xamarin.Forms.Page"/> instance in order to function properly.
    /// </summary>
    public interface IPageAware
    {
        /// <summary>
        /// The <see cref="Xamarin.Forms.Page"/> instance.
        /// </summary>
        Page Page { get; set; } 
    }
}