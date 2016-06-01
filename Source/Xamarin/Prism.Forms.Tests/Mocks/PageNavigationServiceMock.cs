using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationServiceMock : PageNavigationService
    {
        PageNavigationContainerMock _containerMock;

        public PageNavigationServiceMock(PageNavigationContainerMock containerMock, IApplicationProvider applicationProviderMock, ILoggerFacade loggerFacadeMock)
            : base(applicationProviderMock, loggerFacadeMock)
        {
            _containerMock = containerMock;
        }

        protected override Page CreatePage(string name)
        {
            return _containerMock.GetInstance(name) as Page;
        }
    }
}
