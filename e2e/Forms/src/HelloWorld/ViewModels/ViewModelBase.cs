using System;
using System.Threading.Tasks;
using Prism;
using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;

namespace HelloWorld.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IInitialize, IInitializeAsync, IDestructible, IConfirmNavigation, IConfirmNavigationAsync, IPageLifecycleAware, IActiveAware
    {
        private bool active;
        public bool IsActive
        {
            get => active;
            set => SetProperty(ref active, value, () => Console.WriteLine($"{GetType().Name}: IsActive: {value}"));
        }

        public event EventHandler IsActiveChanged;

        public bool CanNavigate(INavigationParameters parameters)
        {
            Console.WriteLine($"{GetType().Name}: {nameof(CanNavigate)}");
            return true;
        }

        public Task<bool> CanNavigateAsync(INavigationParameters parameters)
        {
            Console.WriteLine($"{GetType().Name}: {nameof(CanNavigateAsync)}");
            return Task.FromResult(true);
        }

        public void Destroy()
        {
            Console.WriteLine($"{GetType().Name}: {nameof(Destroy)}");
        }

        public void Initialize(INavigationParameters parameters)
        {
            Console.WriteLine($"{GetType().Name}: {nameof(Initialize)}");
        }

        public Task InitializeAsync(INavigationParameters parameters)
        {
            Console.WriteLine($"{GetType().Name}: {nameof(InitializeAsync)}");
            return Task.CompletedTask;
        }

        public void OnAppearing()
        {
            Console.WriteLine($"{GetType().Name}: {nameof(OnAppearing)}");
        }

        public void OnDisappearing()
        {
            Console.WriteLine($"{GetType().Name}: {nameof(OnDisappearing)}");
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            Console.WriteLine($"{GetType().Name}: {nameof(OnNavigatedFrom)}");
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            Console.WriteLine($"{GetType().Name}: {nameof(OnNavigatedTo)}");
        }
    }
}
