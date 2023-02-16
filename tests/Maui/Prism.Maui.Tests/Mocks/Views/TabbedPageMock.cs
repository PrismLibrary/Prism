using Microsoft.Maui.Controls;
using Prism.Maui.Tests.Navigation.Mocks.Views;

namespace Prism.Maui.Tests.Mocks.Views;

public class TabbedPageMock : TabbedPage, IDestructible, IInitialize, INavigationAware, IPageNavigationEventRecordable
{
    public bool DestroyCalled { get; private set; } = false;
    public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

    public TabbedPageMock() : this(null)
    {
    }

    public TabbedPageMock(PageNavigationEventRecorder recorder)
    {
        //ViewModelLocator.SetAutowireViewModel(this, true);

        Children.Add(new Tab1Mock(recorder) { Title = "Page 1" });
        Children.Add(new Tab2Mock() { Title = "Page 2", BindingContext = null });
        Children.Add(new Tab3Mock(recorder) { Title = "Page 3" });
        Children.Add(new NavigationPageMock(recorder, new ContentPageMock(recorder)) { Title = "Page 4" });
        Children.Add(new NavigationPageMock(recorder, new PageMock()) { Title = "Page 5" });

        PageNavigationEventRecorder = recorder;

        var recordable = BindingContext as IPageNavigationEventRecordable;
        if (recordable != null)
            recordable.PageNavigationEventRecorder = recorder;
    }


    public void Destroy()
    {
        DestroyCalled = true;
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
    }

    public void Initialize(INavigationParameters parameters)
    {
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnInitialized);
    }
}
