namespace Prism.Services
{
    /// <summary>
    /// A service that provides acess to platform-specific implementations of a specified type
    /// </summary>
    public class DependencyService : IDependencyService
    {
        /// <summary>
        /// Returns a platform-specific implementation of a type registered with the Xamarin.Forms.DependencyService
        /// </summary>
        /// <typeparam name="T">The type of class to get</typeparam>
        /// <returns>The class instance</returns>
        public T Get<T>() where T : class
        {
            return Xamarin.Forms.DependencyService.Get<T>();
        }
    }
}
