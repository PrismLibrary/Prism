namespace Prism.AppModel
{
    public interface IApplicationLifecycle
    {
        void OnResume();

        void OnSleep();
    }
}
