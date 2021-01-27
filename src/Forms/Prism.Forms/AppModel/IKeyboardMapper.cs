using Xamarin.Forms;

namespace Prism.AppModel
{
    /// <summary>
    /// An abstraction to map <see cref="KeyboardType"/> to the <see cref="Keyboard"/>;
    /// </summary>
    public interface IKeyboardMapper
    {
        /// <summary>
        /// Maps the <see cref="KeyboardType"/> to a <see cref="Keyboard"/>
        /// </summary>
        /// <param name="keyboardType">The Keyboard type.</param>
        /// <returns>The <see cref="Keyboard"/>.</returns>
        Keyboard Map(KeyboardType keyboardType);
    }
}
