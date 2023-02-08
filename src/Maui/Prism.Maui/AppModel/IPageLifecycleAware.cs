namespace Prism.AppModel;

public interface IPageLifecycleAware
{
    void OnAppearing();
    void OnDisappearing();
}
