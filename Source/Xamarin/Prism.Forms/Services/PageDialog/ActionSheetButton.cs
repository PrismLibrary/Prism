using Prism.Commands;

namespace Prism.Services
{
    public class ActionSheetButton : IActionSheetButton
    {
        #region Implementation of IActionSheetButton

        public virtual bool IsDestroy { get; protected set; }
        public virtual bool IsCancel { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual DelegateCommand Callback { get; protected set; }

        #endregion

        protected ActionSheetButton()
        {

        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "other button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="callback">Callback when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateCancelButton(string text, DelegateCommand callback)
        {
            return CreateButtonInternal(text, callback, false, true);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "destroy button"
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="callback">Callback when button pressed</param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateDestroyButton(string text, DelegateCommand callback)
        {
            return CreateButtonInternal(text, callback, true, false);
        }

        /// <summary>
        /// Create a new instance of <see cref="ActionSheetButton"/> that display as "cancel button"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="callback"></param>
        /// <returns>An instance of <see cref="ActionSheetButton"/></returns>
        public static ActionSheetButton CreateButton(string text, DelegateCommand callback)
        {
            return CreateButtonInternal(text, callback, false, false);
        }

        static ActionSheetButton CreateButtonInternal(string text, DelegateCommand callback, bool isDestroy, bool isCancel)
        {
            return new ActionSheetButton
            {
                Text = text,
                Callback = callback,
                IsDestroy = isDestroy,
                IsCancel = isCancel
            };
        }
    }
}