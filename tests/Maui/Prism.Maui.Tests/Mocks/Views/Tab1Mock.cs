using Prism.Maui.Tests.Mocks;
using Prism.Maui.Tests.Mocks.Views;

namespace Prism.Maui.Tests.Navigation.Mocks.Views;

public class Tab1Mock : ContentPageMock
{
    public Tab1Mock() : this(null)
    {
    }

    public Tab1Mock(PageNavigationEventRecorder recorder) : base(recorder)
    {

    }
}
