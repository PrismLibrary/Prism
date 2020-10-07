using System;
namespace Prism.Services
{
    /// <summary>
    /// Provides a Generic Implementation for IActionSheetButton
    /// </summary>
    public class ActionSheetButton<T> : ActionSheetButtonBase
    {
        protected internal ActionSheetButton()
        {

        }

        /// <summary>
        /// Generic Action to perform
        /// </summary>
        /// <value>The action.</value>
        public Action<T> Action { get; set; }

        /// <summary>
        /// Typed Parameter
        /// </summary>
        /// <value>The parameter.</value>
        public T Parameter { get; set; }

        /// <summary>
        /// Executes the action to take when the button is pressed
        /// </summary>
        protected override void OnButtonPressed() => Action?.Invoke(Parameter);
    }
}
