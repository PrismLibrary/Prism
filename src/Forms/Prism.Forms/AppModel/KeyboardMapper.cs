using System;
using Xamarin.Forms;

namespace Prism.AppModel
{
    /// <summary>
    /// The default implementation of the <see cref="IKeyboardMapper"/>.
    /// </summary>
    public class KeyboardMapper : IKeyboardMapper
    {
        /// <summary>
        /// Maps the <see cref="KeyboardType"/> to a <see cref="Keyboard"/>
        /// </summary>
        /// <param name="keyboardType">The Keyboard type.</param>
        /// <returns>The <see cref="Keyboard"/>.</returns>
        public virtual Keyboard Map(KeyboardType keyboardType)
        {
            return keyboardType switch
            {
                KeyboardType.Chat => Keyboard.Chat,
                KeyboardType.Default => Keyboard.Default,
                KeyboardType.Email => Keyboard.Email,
                KeyboardType.Numeric => Keyboard.Numeric,
                KeyboardType.Plain => Keyboard.Plain,
                KeyboardType.Telephone => Keyboard.Telephone,
                KeyboardType.Text => Keyboard.Text,
                KeyboardType.Url => Keyboard.Url,
                _ => throw new NotImplementedException($"The Keyboard Type value {keyboardType} is not supported. Please create and register an implementation of IKeyboardMapper to support this value.")
            };
        }
    }
}
