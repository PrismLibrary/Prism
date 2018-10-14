using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.Navigation.Xaml
{
    public abstract class NavigationExtensionBase : IMarkupExtension<ICommand>, ICommand
    {
        private IServiceProvider ServiceProvider;

        private BindableObject bindable;
        protected BindableObject Bindable
        {
            get
            {
                if (bindable == null)
                {
                    Initialize();
                }
                return bindable;
            }
            set => bindable = value;
        }

        protected internal bool IsNavigating;

        private Page sourcePage;
        public Page SourcePage
        {
            protected internal get
            {
                if(sourcePage == null)
                {
                    Initialize();
                }

                return sourcePage;
            }
            set => sourcePage = value;
        }

        public bool CanExecute(object parameter) => !IsNavigating;

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            var parameters = parameter.ToNavigationParameters(Bindable);

            IsNavigating = true;
            try
            {
                RaiseCanExecuteChanged();

                var navigationService = Navigation.GetNavigationService(SourcePage);
                await HandleNavigation(parameters, navigationService);
            }
            catch(Exception ex)
            {
                Log(ex);
            }
            finally
            {
                IsNavigating = false;
                RaiseCanExecuteChanged();
            }
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        public ICommand ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            return this;
        }

        protected virtual void Log(Exception ex)
        {
            Xamarin.Forms.Internals.Log.Warning("Warning", $"{GetType().Name} threw an exception");
            Xamarin.Forms.Internals.Log.Warning("Exception", ex.ToString());
        }

        protected abstract Task HandleNavigation(INavigationParameters parameters, INavigationService navigationService);

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private void Initialize()
        {
            var valueTargetProvider = ServiceProvider.GetService<IProvideValueTarget>();

            if (valueTargetProvider == null)
                throw new ArgumentException("The ServiceProvider did not provide a 'IProvideValueTarget'");

            var propertyInfo = valueTargetProvider.GetType()
                                        .GetRuntimeProperties()
                                        .FirstOrDefault(p => p.Name.EndsWith("ParentObjects"));

            if (propertyInfo == null) throw new ArgumentNullException("ParentObjects");
            var parentObjects = ((IEnumerable)propertyInfo.GetValue(valueTargetProvider))
                                                          .Cast<BindableObject>();

            var parentObject = parentObjects.FirstOrDefault(pO => pO.GetType()
                                                                    .GetTypeInfo()
                                                                    .IsSubclassOf(typeof(Page)));

            Bindable = parentObjects.FirstOrDefault();

            if (parentObject == null)
                throw new ArgumentNullException(nameof(parentObject));

            Navigation.SetRaiseCanExecuteChangedInternal(Bindable, RaiseCanExecuteChanged);

            if (sourcePage == null && parentObject is Page providedPage)
            {
                SourcePage = providedPage;

                if(providedPage.Parent is MasterDetailPage mdp 
                    && mdp.Master == providedPage)
                {
                    SourcePage = mdp;
                }
            }
        }
    }
}