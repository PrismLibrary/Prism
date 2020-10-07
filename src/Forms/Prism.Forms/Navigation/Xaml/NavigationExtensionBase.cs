using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Xaml;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    public abstract class NavigationExtensionBase : Prism.Xaml.ParentPageAwareExtension<ICommand>, ICommand
    {
        public static readonly BindableProperty AnimatedProperty =
            BindableProperty.Create(nameof(Animated), typeof(bool), typeof(NavigationExtensionBase), true);

        public static readonly BindableProperty UseModalNavigationProperty =
            BindableProperty.Create(nameof(UseModalNavigation), typeof(bool?), typeof(NavigationExtensionBase), null);

        protected internal bool IsNavigating { get; private set; }

        public bool Animated
        {
            get => (bool)GetValue(AnimatedProperty);
            set => SetValue(AnimatedProperty, value);
        }

        public bool? UseModalNavigation
        {
            get => (bool?)GetValue(UseModalNavigationProperty);
            set => SetValue(UseModalNavigationProperty, value);
        }

        public bool CanExecute(object parameter) => !IsNavigating;

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(TargetElement);

            IsNavigating = true;
            try
            {
                RaiseCanExecuteChanged();

                var navigationService = Navigation.GetNavigationService(SourcePage);
                await HandleNavigation(parameters, navigationService);
            }
            catch (Exception ex)
            {
                Log(ex, parameters);
            }
            finally
            {
                IsNavigating = false;
                RaiseCanExecuteChanged();
            }
        }

        protected override ICommand ProvideValue() =>
            this;

        protected override void OnTargetElementChanged()
        {
            Navigation.SetRaiseCanExecuteChangedInternal(TargetElement, RaiseCanExecuteChanged);
        }

        protected virtual void Log(Exception ex, INavigationParameters parameters)
        {
            Xamarin.Forms.Internals.Log.Warning("Warning", $"{GetType().Name} threw an exception");
            Xamarin.Forms.Internals.Log.Warning("Exception", ex.ToString());
        }

        protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
