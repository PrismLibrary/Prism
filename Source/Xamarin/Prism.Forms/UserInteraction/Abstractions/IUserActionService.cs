using System.Threading.Tasks;

namespace Prism.UserInteraction.Abstractions
{
    /// <summary>
    /// Service to display a view with multiple options.
    /// </summary>
    public interface IUserActionService
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