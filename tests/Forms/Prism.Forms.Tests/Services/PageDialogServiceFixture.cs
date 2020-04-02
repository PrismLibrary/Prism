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
        public void CancelActionSheetButton_WithNoAction_DoNotThrowException()
        {
            var cancel = ActionSheetButton.CreateCancelButton("Foo");
            var ex = Record.Exception(() => cancel.PressButton());
            Assert.Null(ex);
        }

        [Fact]
        public void DestroyActionSheetButton_WithNoAction_DoNotThrowException()
        {
            var destroy = ActionSheetButton.CreateDestroyButton("Foo");
            var ex = Record.Exception(() => destroy.PressButton());
            Assert.Null(ex);
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
