﻿using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class NavigationPageMock : NavigationPage, INavigationAware, IDestructible, IPageNavigationEventRecordable, INavigationPageOptions
    {
        public bool ClearNavigationStackOnNavigation { get; set; } = true;
        public bool DestroyCalled { get; private set; } = false;
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

        public NavigationPageMock() : this(null)
        {
        }

        public NavigationPageMock(PageNavigationEventRecorder recorder) : base(new ContentPageMock(recorder))
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public NavigationPageMock(PageNavigationEventRecorder recorder, Page page) : base(page)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public void Destroy()
        {
            DestroyCalled = true;
            PageNavigationEventRecorder.Record(this, PageNavigationEvent.Destroy);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatingTo);
        }
    }

    public class NavigationPageEmptyMock : NavigationPage, INavigationAware, IDestructible, IPageNavigationEventRecordable
    {
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }
        public NavigationPageEmptyMock() : this(null)
        {
        }

        public NavigationPageEmptyMock(PageNavigationEventRecorder recorder)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public void Destroy()
        {
            PageNavigationEventRecorder.Record(this, PageNavigationEvent.Destroy);
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatingTo);
        }

    }

    public class NavigationPageWithStackMock : NavigationPage
    {
        public NavigationPageWithStackMock() : base()
        {
            var p1 = new ContentPageMock();
            var p2 = new ContentPage();
            var p3 = new ContentPage();

            Navigation.PushAsync(p1);
            p1.Navigation.PushAsync(p2);
            p2.Navigation.PushAsync(p3);
        }
    }

    public class NavigationPageWithStackNoMatchMock : NavigationPage
    {
        public NavigationPageWithStackNoMatchMock() : base()
        {
            var p1 = new ContentPage();
            var p2 = new ContentPage();
            var p3 = new ContentPage();

            Navigation.PushAsync(p1);
            p1.Navigation.PushAsync(p2);
            p2.Navigation.PushAsync(p3);
        }
    }
}
