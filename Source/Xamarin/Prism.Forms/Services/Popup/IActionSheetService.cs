using System.Threading.Tasks;
using Prism.Navigation;

namespace Prism.Services
{
    /// <summary>
    /// Service to display a view with multiple options.
    /// </summary>
    public interface IActionSheetService : IPageAware
    {
        /// <summary>
        /// Display a view with multiple options
        /// </summary>
        /// <param name="title">Title to display in view.</param>
        /// <param name="cancelButton">Text for the cancel button.</param>
        /// <param name="destroyButton">Text for the ok button.</param>
        /// <param name="otherButtons">Text for other buttons.</param>
        /// <returns>Text for the pressed button</returns>
        Task<string> DisplayActionSheet(string title, string cancelButton, string destroyButton, params string[] otherButtons);
    }
}