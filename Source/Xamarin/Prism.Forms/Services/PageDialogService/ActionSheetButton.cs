using System.Windows.Input;

namespace Prism.Services
{
    /// <summary>
    /// Represents a button displayed in <see cref="Prism.Services.IPageDialogService.DisplayActionSheetAsync(string, IActionSheetButton[])"/>
    /// </summary>
    public class ActionSheetButton : IActionSheetButton
    {
        /// <summary>
        /// The button will be used as destroy
        /// </summary>
        public virtual bool IsDestroy { get; protected set; }

        /// <summary>
        /// The button will be used as cancel
        /// </summary>
        public virtual bool IsCancel { get; protected set; }

        /// <summary>
        /// Text to display in the action sheet
        /// </summary>
        public virtual string Text { get; protected set; }

        /// <summary>
        /// Command to execute when button is pressed
        /// </summary>
        public virtual ICommand Command { get; protected set; }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton(string text, ICommand command)
        {
            return CreateButtonInternal(text, command, false, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateDestroyButton(string text, ICommand command)
        {
            return CreateButtonInternal(text, command, true, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateButton(string text, ICommand command)
        {
            return CreateButtonInternal(text, command, false, false);
        }

        static ActionSheetButton CreateButtonInternal(string text, ICommand command, bool isDestroy, bool isCancel)
        {
            return new ActionSheetButton
            {
                Text = text,
                Command = command,
                IsDestroy = isDestroy,
                IsCancel = isCancel
            };
        }
    }
}
