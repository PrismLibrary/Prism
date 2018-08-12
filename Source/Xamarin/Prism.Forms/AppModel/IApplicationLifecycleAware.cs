namespace Prism.AppModel
{
    public interface IApplicationLifecycleAware
    {
        void OnResume();

        void OnSleep();
    }
}
