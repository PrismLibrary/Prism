using Prism.Commands;

namespace Prism.Services
{
    public class ActionSheetButtonBase : IActionSheetButton
    {
        #region Implementation of IActionSheetButton

        public virtual bool IsDestroy { get; protected set; }
        public virtual bool IsCancel { get; protected set; }
        public virtual string Text { get; protected set; }
        public virtual DelegateCommand Callback { get; protected set; }

        #endregion

        public static ActionSheetButtonBase CreateCancelButton(string text, DelegateCommand callback)
        {
            return CreateButtonInternal(text, callback, false, true);
        }

        public static ActionSheetButtonBase CreateDestructiveButton(string text, DelegateCommand callback)
        {
            return CreateButtonInternal(text, callback, true, false);
        }

        public static ActionSheetButtonBase CreateButton(string text, DelegateCommand callback)
        {
            return CreateButtonInternal(text, callback, false, false);
        }

        static ActionSheetButtonBase CreateButtonInternal(string text, DelegateCommand callback, bool isDestroy, bool isCancel)
        {
            return new ActionSheetButtonBase
            {
                Text = text,
                Callback = callback,
                IsDestroy = isDestroy,
                IsCancel = isCancel
            };
        }
    }
}