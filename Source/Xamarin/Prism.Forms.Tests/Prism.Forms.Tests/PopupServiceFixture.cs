using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Forms.Tests.Mocks;
using Prism.Services;
using Xunit;

namespace Prism.Forms.Tests
{
    public class PopupServiceFixture
    {

        [Fact]
        public async Task DisplayActionSheetExtensionNoButtons_ShouldThrowException()
        {
            var service = new PopupServiceMock("cancel");
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(() => service.DisplayActionSheet(null, null));
            Assert.Equal(typeof(ArgumentException), argumentException.GetType());
        }

        [Fact]
        public async Task DisplayActionSheetExtension_CancelButtonPressed()
        {
            var service = new PopupServiceMock("cancel");
            var cancelButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => cancelButtonPressed = true);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            await service.DisplayActionSheet(null, button);
            Assert.True(cancelButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheetExtension_DestroyButtonPressed()
        {
            var service = new PopupServiceMock("destroy");
            var destroyButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => destroyButtonPressed = false);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            var destroyCommand = new DelegateCommand(() => destroyButtonPressed = true);
            var destroyButton = ActionSheetButton.CreateDestructiveButton("destroy", destroyCommand);
            await service.DisplayActionSheet(null, button, destroyButton);
            Assert.True(destroyButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheetExtension_OtherButtonPressed()
        {
            var service = new PopupServiceMock("other");
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheet(null, button);
            Assert.True(buttonPressed);
        }

        [Fact]
        public async Task DisplayActionSheetExtension_NullButtonAndOtherButtonPressed()
        {
            var service = new PopupServiceMock("other");
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheet(null, button, null);
            Assert.True(buttonPressed);
        }
    }
}
