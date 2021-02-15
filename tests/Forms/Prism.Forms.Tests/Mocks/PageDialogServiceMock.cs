using System.Linq;
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

        public override Task DisplayActionSheetAsync(string title, FlowDirection flowDirection, params IActionSheetButton[] buttons)
        {
            foreach (var button in buttons.Where(button => button != null && button.Text.Equals(pressedButton)))
            {
                button.PressButton();
            }
            return Task.FromResult(pressedButton);
        }
    }
}
