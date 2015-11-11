using Prism.Commands;
using Prism.Forms.Tests.Mocks;
using Prism.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Forms.Tests.Services
{
    public class PageDialogServiceFixture
    {
        [Fact]
        public async Task DisplayAlert_AcceptButtonPressed()
        {
            var service = new PageDialogServiceMock("accept");
            var result = await service.DisplayAlert("title", "message", "accept", "cancel");
            Assert.True(result);
        }

        [Fact]
        public async Task DisplayAlert_CancelButtonPressed()
        {
            var service = new PageDialogServiceMock("cancel");
            Assert.False(service.DisplayAlertCalled);
            await service.DisplayAlert("title", "message", "cancel");
            Assert.True(service.DisplayAlertCalled);
        }

        [Fact]
        public async Task DisplayActionSheet_ButtonPressed()
        {
            var service = new PageDialogServiceMock("other");
            var result = await service.DisplayActionSheet("title", "cancel", "destroy", "other");
            Assert.Equal("other", result);            
        }

        [Fact]
        public async Task DisplayActionSheetNoButtons_ShouldThrowException()
        {
            var service = new PageDialogServiceMock("cancel");
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(() => service.DisplayActionSheet(null, null));
            Assert.Equal(typeof(ArgumentException), argumentException.GetType());
        }

        [Fact]
        public async Task DisplayActionSheet_CancelButtonPressed()
        {
            var service = new PageDialogServiceMock("cancel");
            var cancelButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => cancelButtonPressed = true);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            await service.DisplayActionSheet(null, button);
            Assert.True(cancelButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_DestroyButtonPressed()
        {
            var service = new PageDialogServiceMock("destroy");
            var destroyButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => destroyButtonPressed = false);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            var destroyCommand = new DelegateCommand(() => destroyButtonPressed = true);
            var destroyButton = ActionSheetButton.CreateDestroyButton("destroy", destroyCommand);
            await service.DisplayActionSheet(null, button, destroyButton);
            Assert.True(destroyButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_NoButtonPressed()
        {
            var service = new PageDialogServiceMock(null);
            var buttonPressed = false;
            var cancelCommand = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            var destroyCommand = new DelegateCommand(() => buttonPressed = true);
            var destroyButton = ActionSheetButton.CreateDestroyButton("destroy", destroyCommand);
            await service.DisplayActionSheet(null, button, destroyButton);
            Assert.False(buttonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_OtherButtonPressed()
        {
            var service = new PageDialogServiceMock("other");
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheet(null, button);
            Assert.True(buttonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_NullButtonAndOtherButtonPressed()
        {
            var service = new PageDialogServiceMock("other");
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheet(null, button, null);
            Assert.True(buttonPressed);
        }
    }
}
