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
            var instance = _containerMock.GetInstance(name);
            var recodable = instance as IPageNavigationEventRecodable;
            if (recodable != null)
            {
                recodable.PageNavigationEventRecorder = _recorder;
            }

            var page = instance as Page;
            var viewModelMock = page?.BindingContext as IPageNavigationEventRecodable;
            if (viewModelMock != null)
            {
                viewModelMock.PageNavigationEventRecorder = _recorder;
            }

            return page;
        }
    }
}
