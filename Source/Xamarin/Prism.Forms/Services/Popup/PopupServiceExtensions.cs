using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prism.Services
{
    public static class PopupServiceExtensions
    {
        private static readonly Func<IActionSheetButton, bool> DestroyButtonFunc =
            button => button != null && button.IsDestroy;

        private static readonly Func<IActionSheetButton, bool> CanceButtonFunc =
            button => button != null && button.IsCancel;

        // The other buttons are those that are neither destructive nor cancel
        private static readonly Func<IActionSheetButton, bool> OtherButtonsFunc =
            button => button != null && !(button.IsDestroy || button.IsCancel);


        /// <summary>
        /// Display an action sheet with <paramref name="title"/> and <see cref="buttons"/>
        /// </summary>
        /// <para>
        /// The text displayed in the action sheet will be the value for <see cref="IActionSheetButton.Text"/> and when pressed
        /// the <see cref="IActionSheetButton.Callback"/> will be executed.
        /// </para>
        /// <param name="service">Instance of <see cref="IPopupService"/></param>
        /// <param name="title">Text to display in action sheet</param>
        /// <param name="buttons">Buttons displayed in action sheet</param>
        /// <returns></returns>
        public static async Task DisplayActionSheet(this IPopupService service, string title, params IActionSheetButton[] buttons)
        {
            if (buttons == null || buttons.All(b => b == null))
            {
                throw new ArgumentException("At least one button need to be supplied", nameof(buttons));
            }
            var destroyButton = buttons.FirstOrDefault(DestroyButtonFunc);
            var cancelButton = buttons.FirstOrDefault(CanceButtonFunc);
            var otherButtonsText = buttons.Where(OtherButtonsFunc).Select(b => b.Text).ToArray();
            var pressedButton =
                await service.DisplayActionSheet(title, cancelButton?.Text, destroyButton?.Text, otherButtonsText);
            // Find the pressed button and execute the command
            foreach (var button in buttons.Where(button => button != null && pressedButton.Equals(button.Text)))
            {
                await button.Callback.Execute();
                return;
            }
        }
    }
}