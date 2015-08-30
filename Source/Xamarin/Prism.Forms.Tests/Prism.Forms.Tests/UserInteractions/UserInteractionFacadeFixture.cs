using System.Threading.Tasks;
using Prism.Forms.Tests.Mocks.UserInteractions;
using Prism.UserInteraction;
using Xunit;

namespace Prism.Forms.Tests.UserInteractions
{
    public class UserInteractionFacadeFixture
    {

        [Fact]
        public void DisplayActionSheetCancelButtonPressed()
        {
            var service = new UserActionServiceMock("Cancel");
            var facade = new UserInteractionFacade(null, service);
            var actionIndex = facade.DisplayActionSheet("", "Cancel").Result;
            Assert.Equal(0, actionIndex);
        }

        [Fact]
        public void DisplayActionSheetDestroyButtonPressed()
        {
            var service = new UserActionServiceMock("Destroy");
            var facade = new UserInteractionFacade(null, service);
            var actionIndex = facade.DisplayActionSheet("", "Cancel", "Destroy").Result;
            Assert.Equal(1, actionIndex);
        }

        [Fact]
        public void DisplayActionSheetOtherButtonPressedFirstButton()
        {
            var service = new UserActionServiceMock("First");
            var facade = new UserInteractionFacade(null, service);
            var actionIndex = facade.DisplayActionSheet("", "Cancel", "Destroy", "First").Result;
            Assert.Equal(2, actionIndex);
        }

        [Fact]
        public void DisplayActionSheetOtherButtonPressedLastButton()
        {
            var service = new UserActionServiceMock("Last");
            var facade = new UserInteractionFacade(null, service);
            var actionIndex = facade.DisplayActionSheet("", "Cancel", "Destroy", "First", "Last").Result;
            Assert.Equal(3, actionIndex);
        }

        [Fact]
        public void DisplayActionSheetOtherButtonPressedNullButton()
        {
            var service = new UserActionServiceMock("Last");
            var facade = new UserInteractionFacade(null, service);
            var actionIndex = facade.DisplayActionSheet("", "Cancel", "Destroy", "First", null, "Last").Result;
            Assert.Equal(4, actionIndex);
        }


    }
}
