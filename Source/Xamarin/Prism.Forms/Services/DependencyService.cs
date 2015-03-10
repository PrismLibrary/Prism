namespace Prism.Services
{
    public class DependencyService : IDependencyService
    {
        public T Get<T>() where T : class
        {
            return Xamarin.Forms.DependencyService.Get<T>();
        }
    }
}
