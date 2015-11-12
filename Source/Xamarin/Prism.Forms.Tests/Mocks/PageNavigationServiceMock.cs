using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationServiceMock : PageNavigationService
    {
        PageNavigationContainerMock _containerMock;

        public PageNavigationServiceMock(PageNavigationContainerMock containerMock)
        {
            _containerMock = containerMock;
        }

        protected override Page CreatePage(string name)
        {
            return _containerMock.GetInstance(name) as Page;
        }
    }
}
