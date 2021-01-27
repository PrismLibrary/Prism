namespace Prism.AppModel
{
    /// <summary>
    /// Keyboard type
    /// </summary>
    public enum KeyboardType
    {
        /// <summary>
        /// Gets an instance of type "ChatKeyboard".
        /// </summary>
        Chat,

        /// <summary>
        /// Gets an instance of type "Keyboard".
        /// </summary>
        Default,

        /// <summary>
        /// Gets an instance of type "EmailKeyboard".
        /// </summary>
        Email,

        /// <summary>
        /// Gets an instance of type "NumericKeyboard".
        /// </summary>
        Numeric,

        /// <summary>
        /// Returns a new keyboard with None Xamarin.Forms.KeyboardFlags.
        /// </summary>
        Plain,

        /// <summary>
        /// Gets an instance of type "TelephoneKeyboard".
        /// </summary>
        Telephone,

        /// <summary>
        /// Gets an instance of type "TextKeyboard".
        /// </summary>
        Text,

        /// <summary>
        /// Gets an instance of type "UrlKeyboard".
        /// </summary>
        Url,
    }
}
