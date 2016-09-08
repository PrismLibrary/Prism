using System;
using System.Threading.Tasks;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    public class MultiPageNavigationBehavior<T> : BehaviorBase<MultiPage<T>> where T : Page
    {
        private ILoggerFacade _logger { get; }

        private Page _lastSelectedPage;

        public MultiPageNavigationBehavior() : base() { }

        public MultiPageNavigationBehavior( ILoggerFacade logger ) : base()
        {
            _logger = logger;
        }

        protected override void OnAttachedTo(MultiPage<T> bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.CurrentPageChanged += CurrentPageChangedHandler;
        }

        protected override void OnDetachingFrom(MultiPage<T> bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.CurrentPageChanged -= CurrentPageChangedHandler;
        }

        private async void CurrentPageChangedHandler(object sender, EventArgs args)
        {
            NavigationParameters parameters = GetNavigationParameters();

            if( _lastSelectedPage == null )
                _lastSelectedPage = AssociatedObject.CurrentPage;

            // Step 1: Call Navigated From on the last Page
            await NavigationAwareHandler( _lastSelectedPage, parameters, navigatedTo: false );

            // Step 2: Call Navigated From on the last Page's View Model
            await NavigationAwareHandler( _lastSelectedPage.BindingContext, parameters, navigatedTo: false );

            // Step 3: Call Navigated To on the new Page
            await NavigationAwareHandler( AssociatedObject.CurrentPage, parameters, navigatedTo: true );

            // Step 4: Call Navigated To on the new Page's View Model
            await NavigationAwareHandler( AssociatedObject.CurrentPage.BindingContext, parameters, navigatedTo: true );

            // Step 5: Update the last selected page to the current page
            _lastSelectedPage = AssociatedObject.CurrentPage;
        }

        private NavigationParameters GetNavigationParameters()
        {
            var parameters = new NavigationParameters();
            var view = AssociatedObject as IInternalNavigationParent;
            var vm = AssociatedObject.BindingContext as IInternalNavigationParent;

            if( view != null ) parameters = MergeParameters( parameters, view.SharedParameters );
            if( vm != null ) parameters = MergeParameters( parameters, vm.SharedParameters );

            return parameters;
        }

        private NavigationParameters MergeParameters( NavigationParameters starting, NavigationParameters newParameters )
        {
            if( starting == null ) starting = new NavigationParameters();

            if( newParameters != null )
            {
                foreach( var param in newParameters )
                    starting[ param.Key ] = param.Value;
            }

            return starting;
        }

        private async Task NavigationAwareHandler( object obj, NavigationParameters parameters, bool navigatedTo )
        {
            try
            {
                if( obj == null || ( _lastSelectedPage == AssociatedObject.CurrentPage && navigatedTo == false ) )
                    return;

                var awareObj = obj as IMultiPageNavigationAware;
                var awareAsyncObj = obj as IMultiPageNavigationAwareAsync;

                if( navigatedTo )
                {
                    awareObj?.OnInternalNavigatedTo( parameters );
                    if( awareAsyncObj != null )
                        await awareAsyncObj.OnInternalNavigatedTo( parameters );
                }
                else
                {
                    awareObj?.OnInternalNavigatedFrom( parameters );
                    if( awareAsyncObj != null )
                        await awareAsyncObj.OnInternalNavigatedFrom( parameters );
                }
            }
            catch( Exception e )
            {
                _logger?.Log( e.ToString(), Category.Exception, Priority.None );
            }
        }
    }
}

