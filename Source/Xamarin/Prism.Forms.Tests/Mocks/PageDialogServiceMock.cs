using Prism.Common;
using Prism.Services;
using System.Threading.Tasks;

namespace Prism.Forms.Tests.Mocks
{
    public class PageDialogServiceMock : PageDialogService
    {
        private readonly string pressedButton;

        /// <summary>
        /// Create an instance of <see cref="PageDialogServiceMock"/> with the pressed button on any alert/sheet is <paramref name="pressedButton"/>
        /// </summary>
        /// <param name="pressedButton"></param>
        public PageDialogServiceMock(string pressedButton, IApplicationProvider applicationProvider)
            : base(applicationProvider)
        {
            this.pressedButton = pressedButton;
        }

        public override Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return Task.FromResult(pressedButton);
        }
    }
}
