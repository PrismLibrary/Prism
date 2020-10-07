using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation.TabbedPages;
using Prism.Xaml;
using Xamarin.Forms;

namespace Prism.Navigation.Xaml
{
    [ContentProperty(nameof(Name))]
    public class SelectTabExtension : ParentPageAwareExtension<ICommand>, ICommand
    {
        public static readonly BindableProperty NameProperty =
               BindableProperty.Create(nameof(Name), typeof(string), typeof(NavigateToExtension), null);

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        protected internal bool IsNavigating { get; private set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => !IsNavigating;

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

        protected async Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService)
        {
            var result = await navigationService.SelectTabAsync(Name, parameters);
            if (!result.Success)
            {
                Log(result.Exception, parameters);
            }
        }

        protected override ICommand ProvideValue() =>
            this;

        protected virtual void Log(Exception ex, INavigationParameters parameters)
        {
            Xamarin.Forms.Internals.Log.Warning("Warning", $"{GetType().Name} threw an exception");
            Xamarin.Forms.Internals.Log.Warning("Exception", ex.ToString());
        }

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
