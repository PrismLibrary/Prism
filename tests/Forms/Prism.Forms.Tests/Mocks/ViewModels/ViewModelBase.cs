using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.Forms.Tests.Mocks.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, IInitializeAsync, INavigationAware, IDestructible, IPageNavigationEventRecordable
    {
        public INavigationParameters NavigatedToParameters { get; private set; }
        public INavigationParameters NavigatedFromParameters { get; private set; }
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

        public bool OnNavigatedToCalled { get; private set; } = false;

        public bool InitializeCalled { get; private set; } = false;

        public bool InitializeAsyncCalled { get; private set; } = false;

        public bool OnNavigatedFromCalled { get; private set; } = false;

        public bool DestroyCalled { get; private set; } = false;

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            OnNavigatedFromCalled = true;
            NavigatedFromParameters = parameters;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            OnNavigatedToCalled = true;
            NavigatedToParameters = parameters;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
        }

        public void Initialize(INavigationParameters parameters)
        {
            InitializeCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnInitialized);
        }

        public Task InitializeAsync(INavigationParameters parameters)
        {
            InitializeAsyncCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnInitializedAsync);
            return Task.CompletedTask;
        }

        public void Destroy()
        {
            DestroyCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
        }
    }
}
