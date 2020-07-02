using Prism.Common;

namespace Prism.Services.Dialogs
{
    /// <summary>
    /// Represents Dialog parameters.
    /// </summary>
    /// <remarks>
    /// This class can be used to to pass object parameters during the showing and closing of Dialogs.
    /// </remarks>
    public class DialogParameters : ParametersBase, IDialogParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogParameters"/> class.
        /// </summary>
        public DialogParameters() : base() { }

        /// <summary>
        /// Constructs a list of parameters.
        /// </summary>
        /// <param name="query">Query string to be parsed.</param>
        public DialogParameters(string query) : base(query) { }
    }
}
