using System;
using System.Windows.Input;
using Prism.Commands;

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
        /// Command Parameter to pass to the Command when button is pressed
        /// </summary>
        public virtual object CommandParameter { get; protected set; }

        /// <summary>
        /// Forces the use of the command parameter
        /// </summary>
        protected bool UseCommandParameter { get; set; }

        /// <summary>
        /// Executes the Command when the button has been pressed
        /// </summary>
        public virtual void PressButton()
        {
            if(Command == null) return;

            var parameter = UseCommandParameter ? CommandParameter : Text;

            if(Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton(string text)
        {
            return CreateButtonInternal(text, null, null, false, true, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton(string text, ICommand command)
        {
            return CreateButtonInternal(text, command, null, false, true, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton(string text, Action action)
        {
            return CreateButtonInternal(text, new DelegateCommand(action), null, false, true, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <param name="commandParameter">Command Parameter to pass the command when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton(string text, ICommand command, object commandParameter)
        {
            return CreateButtonInternal(text, command, commandParameter, false, true, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <param name="commandParameter">Command Parameter to pass the command when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton<T>(string text, Action<T> action, T commandParameter)
        {
            return CreateButtonInternal(text, new DelegateCommand<T>(action), commandParameter, false, true, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateDestroyButton(string text, ICommand command)
        {
            return CreateButtonInternal(text, command, null, true, false, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateDestroyButton(string text, Action action)
        {
            return CreateButtonInternal(text, new DelegateCommand(action), null, true, false, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <param name="commandParameter">Command Parameter to pass the command when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateDestroyButton(string text, ICommand command, object commandParameter)
        {
            return CreateButtonInternal(text, command, commandParameter, true, false, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <param name="commandParameter">Command Parameter to pass the command when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateDestroyButton<T>(string text, Action<T> action, T commandParameter)
        {
            return CreateButtonInternal(text, new DelegateCommand<T>(action), commandParameter, true, false, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateButton(string text, ICommand command)
        {
            return CreateButtonInternal(text, command, null, false, false, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateButton(string text, Action action)
        {
            return CreateButtonInternal(text, new DelegateCommand(action), null, false, false, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="command">Command to execute when button pressed</param>
        /// <param name="commandParameter">Command Parameter to pass the command when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateButton(string text, ICommand command, object commandParameter)
        {
            return CreateButtonInternal(text, command, commandParameter, false, false, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <param name="commandParameter">Command Parameter to pass the command when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateButton<T>(string text, Action<T> action, T commandParameter)
        {
            return CreateButtonInternal(text, new DelegateCommand<T>(action), commandParameter, false, false, true);
        }

        static ActionSheetButton CreateButtonInternal(string text, ICommand command, object commandParameter, bool isDestroy, bool isCancel, bool useCommandParameter)
        {
            return new ActionSheetButton
            {
                Text = text,
                Command = command,
                CommandParameter = commandParameter,
                IsDestroy = isDestroy,
                IsCancel = isCancel,
                UseCommandParameter = useCommandParameter
            };
        }
    }
}
