using System.Collections.Generic;
using System.Threading.Tasks;
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

        public bool HasProcessedCustomNavigation { get; private set; }

        protected override Page CreatePage(string name)
        {
            return _containerMock.GetInstance(name) as Page;
        }

        protected override Task ProcessNavigationForPage(Page currentPage, string nextSegment, Queue<string> segments,
            NavigationParameters navigationParameters, bool? useModalNavigation, bool animated)
        {
            HasProcessedCustomNavigation = true;
            return Task.FromResult(0);
        }
    }
}
