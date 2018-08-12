using System;
namespace Prism.AppModel
{
    public interface IPageLifecycleAware
    {
        void OnAppearing();
        void OnDisappearing();
    }
}
