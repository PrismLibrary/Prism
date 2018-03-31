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
        public async Task DisplayActionSheet_CancelButtonPressed_UsingAction()
        {
            await DisplayActionSheet_PressButton_UsingAction("cancel");
        }

        [Fact]
        public async Task DisplayActionSheet_CancelButtonPressed_UsingGenericAction()
        {
            await DisplayActionSheet_PressButton_UsingGenericAction("cancel");
        }

        [Fact]
        public async Task DisplayActionSheet_DestroyButtonPressed_UsingAction()
        {
            await DisplayActionSheet_PressButton_UsingAction("destroy");
        }

        [Fact]
        public async Task DisplayActionSheet_DestroyButtonPressed_UsingGenericAction()
        {
            await DisplayActionSheet_PressButton_UsingGenericAction("destroy");
        }

        [Fact]
        public async Task DisplayActionSheet_OtherButtonPressed_UsingAction()
        {
            await DisplayActionSheet_PressButton_UsingAction("other");
        }

        [Fact]
        public async Task DisplayActionSheet_OtherButtonPressed_UsingGenericAction()
        {
            await DisplayActionSheet_PressButton_UsingGenericAction("other");
        }

        [Fact]
        public async Task DisplayActionSheet_NoButtonPressed_UsingAction()
        {
            await DisplayActionSheet_PressButton_UsingAction(null);
        }

        [Fact]
        public async Task DisplayActionSheet_NoButtonPressed_UsingGenericAction()
        {
            await DisplayActionSheet_PressButton_UsingGenericAction(null);
        }

        #region Obsolete ActionSheetButton using Commands

        [Fact]
        public async Task DisplayActionSheet_CancelButtonPressed_UsingGenericCommand()
        {
            await DisplayActionSheet_PressButton_UsingGenericCommand("cancel");
        }

        [Fact]
        public async Task DisplayActionSheet_DestroyButtonPressed_UsingGenericCommand()
        {
            await DisplayActionSheet_PressButton_UsingGenericCommand("destroy");
        }

        [Fact]
        public async Task DisplayActionSheet_OtherButtonPressed_UsingGenericCommand()
        {
            await DisplayActionSheet_PressButton_UsingGenericCommand("other");
        }

        [Fact]
        public async Task DisplayActionSheet_NoButtonPressed_UsingGenericCommand()
        {
            await DisplayActionSheet_PressButton_UsingGenericCommand(null);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public async Task DisplayActionSheet_CancelButtonPressed_UsingCommand()
        {
            var service = new PageDialogServiceMock("cancel", _applicationProvider);
            var cancelButtonPressed = false;
            var cancelCommand = new DelegateCommand(() => cancelButtonPressed = true);
            var button = ActionSheetButton.CreateCancelButton("cancel", cancelCommand);
            await service.DisplayActionSheetAsync(null, button);
            Assert.True(cancelButtonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_DestroyButtonPressed_UsingCommand()
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
        public async Task DisplayActionSheet_NoButtonPressed_UsingCommand()
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
        public async Task DisplayActionSheet_OtherButtonPressed_UsingCommand()
        {
            var service = new PageDialogServiceMock("other", _applicationProvider);
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheetAsync(null, button);
            Assert.True(buttonPressed);
        }

        [Fact]
        public async Task DisplayActionSheet_NullButtonAndOtherButtonPressed_UsingCommand()
        {
            var service = new PageDialogServiceMock("other", _applicationProvider);
            var buttonPressed = false;
            var command = new DelegateCommand(() => buttonPressed = true);
            var button = ActionSheetButton.CreateButton("other", command);
            await service.DisplayActionSheetAsync(null, button, null);
            Assert.True(buttonPressed);
        }

        private async Task DisplayActionSheet_PressButton_UsingGenericCommand(string text)
        {
            var service = new PageDialogServiceMock(text, _applicationProvider);
            var cancelButtonModel = new ButtonModel();
            var destroyButtonModel = new ButtonModel();
            var otherButtonModel = new ButtonModel();
            var btns = new IActionSheetButton[]
            {
                ActionSheetButton.CreateButton("other", new DelegateCommand<ButtonModel>(OnButtonPressed), otherButtonModel),
                ActionSheetButton.CreateCancelButton("cancel", new DelegateCommand<ButtonModel>(OnButtonPressed), cancelButtonModel),
                ActionSheetButton.CreateDestroyButton("destroy", new DelegateCommand<ButtonModel>(OnButtonPressed), destroyButtonModel)
            };
            await service.DisplayActionSheetAsync( null, btns );

            switch( text )
            {
                case "other":
                    Assert.False(cancelButtonModel.ButtonPressed);
                    Assert.False(destroyButtonModel.ButtonPressed);
                    Assert.True(otherButtonModel.ButtonPressed);
                    break;
                case "cancel":
                    Assert.True(cancelButtonModel.ButtonPressed);
                    Assert.False(destroyButtonModel.ButtonPressed);
                    Assert.False(otherButtonModel.ButtonPressed);
                    break;
                case "destroy":
                    Assert.False(cancelButtonModel.ButtonPressed);
                    Assert.True(destroyButtonModel.ButtonPressed);
                    Assert.False(otherButtonModel.ButtonPressed);
                    break;
                default:
                    Assert.False(cancelButtonModel.ButtonPressed);
                    Assert.False(destroyButtonModel.ButtonPressed);
                    Assert.False(otherButtonModel.ButtonPressed);
                    break;
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete

        #endregion Obsolete ActionSheetButton using Commands

        private async Task DisplayActionSheet_PressButton_UsingAction(string text)
        {
            var service = new PageDialogServiceMock(text, _applicationProvider);
            bool cancelButtonPressed = false;
            bool destroyButtonPressed = false;
            bool otherButtonPressed = false;
            var btns = new IActionSheetButton[]
            {
                ActionSheetButton.CreateButton("other", () => otherButtonPressed = true),
                ActionSheetButton.CreateCancelButton("cancel", () => cancelButtonPressed = true),
                ActionSheetButton.CreateDestroyButton("destroy", () => destroyButtonPressed = true)
            };
            await service.DisplayActionSheetAsync(null, btns);

            switch(text)
            {
                case "other":
                    Assert.True(otherButtonPressed);
                    Assert.False(cancelButtonPressed);
                    Assert.False(destroyButtonPressed);
                    break;
                case "cancel":
                    Assert.False(otherButtonPressed);
                    Assert.True(cancelButtonPressed);
                    Assert.False(destroyButtonPressed);
                    break;
                case "destroy":
                    Assert.False(otherButtonPressed);
                    Assert.False(cancelButtonPressed);
                    Assert.True(destroyButtonPressed);
                    break;
                default:
                    Assert.False(otherButtonPressed);
                    Assert.False(cancelButtonPressed);
                    Assert.False(destroyButtonPressed);
                    break;
            }
        }

        private class ButtonModel
        {
            public bool ButtonPressed { get; set; }
        }

        private void OnButtonPressed(ButtonModel model) => 
            model.ButtonPressed = true;

        private async Task DisplayActionSheet_PressButton_UsingGenericAction(string text)
        {
            var service = new PageDialogServiceMock(text, _applicationProvider);
            var cancelButtonModel = new ButtonModel();
            var destroyButtonModel = new ButtonModel();
            var otherButtonModel = new ButtonModel();
            var btns = new IActionSheetButton[]
            {
                ActionSheetButton.CreateButton("other", OnButtonPressed, otherButtonModel),
                ActionSheetButton.CreateCancelButton("cancel", OnButtonPressed, cancelButtonModel),
                ActionSheetButton.CreateDestroyButton("destroy", OnButtonPressed, destroyButtonModel)
            };
            await service.DisplayActionSheetAsync(null, btns);

            switch(text)
            {
                case "other":
                    Assert.True(otherButtonModel.ButtonPressed);
                    Assert.False(cancelButtonModel.ButtonPressed);
                    Assert.False(destroyButtonModel.ButtonPressed);
                    break;
                case "cancel":
                    Assert.False(otherButtonModel.ButtonPressed);
                    Assert.True(cancelButtonModel.ButtonPressed);
                    Assert.False(destroyButtonModel.ButtonPressed);
                    break;
                case "destroy":
                    Assert.False(otherButtonModel.ButtonPressed);
                    Assert.False(cancelButtonModel.ButtonPressed);
                    Assert.True(destroyButtonModel.ButtonPressed);
                    break;
                default:
                    Assert.False(otherButtonModel.ButtonPressed);
                    Assert.False(cancelButtonModel.ButtonPressed);
                    Assert.False(destroyButtonModel.ButtonPressed);
                    break;
            }
        }
    }
}
