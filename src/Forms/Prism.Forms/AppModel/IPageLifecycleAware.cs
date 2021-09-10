using System;
namespace Prism.AppModel
{
    /// <summary>
    /// An interface for using the page lifecycle events
    /// </summary>
    public interface IPageLifecycleAware
    {
        void OnAppearing();
        void OnDisappearing();
    }
}
