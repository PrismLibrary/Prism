using Xamarin.Forms;

namespace HelloWorld
{
    public class App : Application
    {
        public App()
        {
            Bootstrapper bs = new Bootstrapper();
            bs.Run(this);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
