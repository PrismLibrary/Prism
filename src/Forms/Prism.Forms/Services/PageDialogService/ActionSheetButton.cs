using System;
using System.Windows.Input;

namespace Prism.Services
{
    /// <summary>
    /// Represents a button displayed in <see cref="Prism.Services.IPageDialogService.DisplayActionSheetAsync(string, IActionSheetButton[])"/>
    /// </summary>
    public class ActionSheetButton : ActionSheetButtonBase
    {
        protected internal ActionSheetButton()
        {

        }

        /// <summary>
        /// Action to perform when the button is pressed
        /// </summary>
        /// <value>The action.</value>
        public Action Action { get; protected set; }

        /// <summary>
        /// Executes the action to take when the button is pressed
        /// </summary>
        protected override void OnButtonPressed() => Action?.Invoke();

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateCancelButton(string text) => CreateCancelButton(text, default);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateCancelButton(string text, Action action) =>
            CreateButtonInternal(text, action, isCancel: true);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <param name="parameter">Parameter to pass the Action when the button is pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateCancelButton<T>(string text, Action<T> action, T parameter) =>
            CreateButtonInternal(text, action, parameter, isCancel: true);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateDestroyButton(string text) =>
            CreateDestroyButton(text, default);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateDestroyButton(string text, Action action) =>
            CreateButtonInternal(text, action, isDestroy: true);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <param name="parameter">Parameter to pass the action when the button is pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateDestroyButton<T>(string text, Action<T> action, T parameter) =>
            CreateButtonInternal(text, action, parameter, isDestroy: true);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateButton(string text, Action action) =>
            CreateButtonInternal(text, action);

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="action">Action to execute when button pressed</param>
        /// <param name="parameter">Parameter to pass the action when the button is pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static IActionSheetButton CreateButton<T>(string text, Action<T> action, T parameter) =>
            CreateButtonInternal(text, action, parameter);

        private static IActionSheetButton CreateButtonInternal(string text, Action action, bool isCancel = false, bool isDestroy = false) =>
            new ActionSheetButton()
            {
                Text = text,
                Action = action,
                IsCancel = isCancel,
                IsDestroy = isDestroy,
            };

        private static IActionSheetButton CreateButtonInternal<T>(string text, Action<T> action, T parameter, bool isCancel = false, bool isDestroy = false) =>
            new ActionSheetButton<T>()
            {
                Text = text,
                Action = action,
                Parameter = parameter,
                IsCancel = isCancel,
                IsDestroy = isDestroy,
            };
    }
}
