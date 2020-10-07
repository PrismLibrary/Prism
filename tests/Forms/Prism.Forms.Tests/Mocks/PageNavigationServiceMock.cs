using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks
{
    public class PageNavigationServiceMock : PageNavigationService
    {
        IContainerExtension _containerMock;
        PageNavigationEventRecorder _recorder;

        public PageNavigationServiceMock(IContainerExtension containerMock, IApplicationProvider applicationProviderMock, PageNavigationEventRecorder recorder = null)
            : base(containerMock, applicationProviderMock, new PageBehaviorFactory())
        {
            _containerMock = containerMock;
            _recorder = recorder;
        }

        protected override Page CreatePage(string name)
        {
            var page = _containerMock.Resolve<object>(name) as Page;

            PageUtilities.InvokeViewAndViewModelAction<IPageNavigationEventRecordable>(
                page,
                x => x.PageNavigationEventRecorder = _recorder);

            return page;
        }
    }
}
