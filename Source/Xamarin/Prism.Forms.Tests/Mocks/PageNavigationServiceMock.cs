using Prism.Common;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationServiceMock : PageNavigationService
    {
        PageNavigationContainerMock _containerMock;
        PageNavigationEventRecorder _recorder;

        public PageNavigationServiceMock(PageNavigationContainerMock containerMock, IApplicationProvider applicationProviderMock, ILoggerFacade loggerFacadeMock, PageNavigationEventRecorder recorder = null)
            : base(applicationProviderMock, loggerFacadeMock)
        {
            _containerMock = containerMock;
            _recorder = recorder;
        }

        protected override Page CreatePage(string name)
        {
            var page = _containerMock.GetInstance(name) as Page;

            PageUtilities.InvokeViewAndViewModelAction<IPageNavigationEventRecordable>(
                page, 
                x => x.PageNavigationEventRecorder = _recorder);

            return page;
        }
    }
}
