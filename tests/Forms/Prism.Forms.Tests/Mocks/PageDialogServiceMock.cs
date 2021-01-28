using System.Threading.Tasks;
using Moq;
using Prism.AppModel;
using Prism.Common;
using Prism.Services;

namespace Prism.Forms.Tests.Mocks
{
    public class PageDialogServiceMock : PageDialogService
    {
        private static IKeyboardMapper KeyboardMapper()
        {
            var mock = new Mock<IKeyboardMapper>();
            mock.Setup(x => x.Map(It.IsAny<KeyboardType>()))
                .Returns(() => Xamarin.Forms.Keyboard.Default);
            return mock.Object;
        }

        private readonly string pressedButton;

        /// <summary>
        /// Create an instance of <see cref="PageDialogServiceMock"/> with the pressed button on any alert/sheet is <paramref name="pressedButton"/>
        /// </summary>
        /// <param name="pressedButton"></param>
        public PageDialogServiceMock(string pressedButton, IApplicationProvider applicationProvider)
            : base(applicationProvider, KeyboardMapper())
        {
            this.pressedButton = pressedButton;
        }

        public override Task<string> DisplayActionSheetAsync(string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return Task.FromResult(pressedButton);
        }
    }
}
