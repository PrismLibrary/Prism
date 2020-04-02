using Prism.Forms.Tests.Mocks;
using Prism.Forms.Tests.Mocks.Views;

namespace Prism.Forms.Tests.Navigation.Mocks.Views
{
    public class Tab1Mock : ContentPageMock
    {
        public Tab1Mock() : this(null)
        {
        }

        public Tab1Mock(PageNavigationEventRecorder recorder) : base (recorder)
        {
            
        }
    }
}
