using Prism.Commands;
using Prism.Common;
using Prism.Forms.Tests.Mocks;
using Prism.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Forms.Tests.Services
{
    public class PageDialogServiceFixture
    {
        IApplicationProvider _applicationProvider;

        public PageDialogServiceFixture()
        {
            _applicationProvider = new ApplicationProviderMock();
        }

        [Fact]
        public async Task DisplayActionSheetNoButtons_ShouldThrowException()
        {
            var service = new PageDialogServiceMock("cancel", _applicationProvider);
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(() => service.DisplayActionSheetAsync(null, null));
            Assert.Equal(typeof(ArgumentException), argumentException.GetType());
        }

        [Fact]
        public async Task DisplayActionSheet_CancelButtonPressed()
        {
            var service = new PageDialogServiceMock("cancel", _applicationProvider);
            var cancelButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => cancelButtonPressed = true);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            await service.DisplayActionSheetAsync(null, button);
            Assert.True(cancelButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_DestroyButtonPressed()
        {
            var service = new PageDialogServiceMock("destroy", _applicationProvider);
            var destroyButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => destroyButtonPressed = false);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            var destroyCommand = new DelegateCommand(() => destroyButtonPressed = true);
            var destroyButton = ActionSheetButton.CreateDestroyButton("destroy", destroyCommand);
            await service.DisplayActionSheetAsync(null, button, destroyButton);
            Assert.True(destroyButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_NoButtonPressed()
        {
            var service = new PageDialogServiceMock(null, _applicationProvider);
            var buttonPressed = false;
            var cancelCommand = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            var destroyCommand = new DelegateCommand(() => buttonPressed = true);
            var destroyButton = ActionSheetButton.CreateDestroyButton("destroy", destroyCommand);
            await service.DisplayActionSheetAsync(null, button, destroyButton);
            Assert.False(buttonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_OtherButtonPressed()
        {
            var service = new PageDialogServiceMock("other", _applicationProvider);
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheetAsync(null, button);
            Assert.True(buttonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_NullButtonAndOtherButtonPressed()
        {
            var service = new PageDialogServiceMock("other", _applicationProvider);
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheetAsync(null, button, null);
            Assert.True(buttonPressed);
        }
    }
}
