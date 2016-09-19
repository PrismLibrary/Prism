using System;
using System.Threading.Tasks;
using Prism.Logging;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    /// <summary>
    /// Provides base generic implementation for MultiPageNavigationBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiPageNavigationBehavior<T> : BehaviorBase<MultiPage<T>> where T : Page
    {
        /// <summary>
        /// Prism Logger
        /// </summary>
        protected ILoggerFacade _logger { get; }

        /// <summary>
        /// The last selected Page
        /// </summary>
        protected T _lastSelectedPage;

        /// <summary>
        /// Provides base MultiPageNavigationBehavior without logging
        /// </summary>
        public MultiPageNavigationBehavior() { }

        /// <summary>
        /// Provides base MultiPageNavigationBehavior with logging
        /// </summary>
        /// <param name="logger"></param>
        public MultiPageNavigationBehavior( ILoggerFacade logger )
        {
            _logger = logger;
        }

        /// <inheritDoc/>
        protected override void OnAttachedTo(MultiPage<T> bindable)
        {
            bindable.CurrentPageChanged += CurrentPageChangedHandler;
            bindable.Appearing += RootPageAppearingHandler;
            bindable.Disappearing += RootPageDisappearingHandler;
            base.OnAttachedTo(bindable);
        }

        /// <inheritDoc/>
        protected override void OnDetachingFrom(MultiPage<T> bindable)
        {
            bindable.CurrentPageChanged -= CurrentPageChangedHandler;
            bindable.Appearing -= RootPageAppearingHandler;
            bindable.Disappearing -= RootPageDisappearingHandler;
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// Event Handler for the MultiPage CurrentPageChanged event
        /// </summary>
        /// <param name="sender">The MultiPage</param>
        /// <param name="e">Event Args</param>
        protected void CurrentPageChangedHandler(object sender, EventArgs e)
        {
            NavigationParameters parameters = GetNavigationParameters();

            if( _lastSelectedPage == null )
                _lastSelectedPage = AssociatedObject.CurrentPage;

            HandleLastPageNavigation( navigatedTo: false, parameters: parameters ).ContinueWith( _ => { return; } );

            HandleNavigationToPage( AssociatedObject.CurrentPage, navigatedTo: true, parameters: parameters ).ContinueWith( _ => { return; } );

            _lastSelectedPage = AssociatedObject.CurrentPage;
        }

        /// <summary>
        /// Event Handler for the MultiPage Appearing event
        /// </summary>
        /// <param name="sender">The MultiPage Appearing</param>
        /// <param name="e">Event Args</param>
        protected void RootPageAppearingHandler( object sender, EventArgs e )
        {
            if( _lastSelectedPage == null )
            {
                _lastSelectedPage = AssociatedObject.CurrentPage;

                HandleLastPageNavigation( navigatedTo: true ).ContinueWith( _ => { return; } );
            }
        }

        /// <summary>
        /// Event Handler for the MultiPage Disappearing event
        /// </summary>
        /// <param name="sender">The MultiPage Disappearing</param>
        /// <param name="e">Event Args</param>
        protected void RootPageDisappearingHandler( object sender, EventArgs e )
        {
            HandleLastPageNavigation( navigatedTo: false ).ContinueWith( _ => { return; } );
        }
        
        /// <summary>
        /// Gets the NavigationParameters
        /// </summary>
        /// <returns></returns>
        protected NavigationParameters GetNavigationParameters()
        {
            var parameters = new NavigationParameters();
            var view = AssociatedObject as IInternalNavigationParent;
            var vm = AssociatedObject.BindingContext as IInternalNavigationParent;

            if( view != null ) parameters = MergeParameters( parameters, view.SharedParameters );
            if( vm != null ) parameters = MergeParameters( parameters, vm.SharedParameters );

            return parameters;
        }

        // This should be replaced with a function in NavigationParameters
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

        /// <summary>
        /// Handles the Navigation for the Last Page
        /// </summary>
        /// <param name="navigatedTo"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected async Task HandleLastPageNavigation( bool navigatedTo, NavigationParameters parameters = null )
        {
            if( parameters == null )
                parameters = GetNavigationParameters();

            await HandleNavigationToPage( _lastSelectedPage, navigatedTo, parameters );
        }

        /// <summary>
        /// Handles the specified type of Navigation (From/To) for the Page object provided.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="navigatedTo">Is the object being Navigated To or From</param>
        /// <param name="parameters">Navigation Parameters to pass the object</param>
        /// <returns></returns>
        protected async Task HandleNavigationToPage( Page page, bool navigatedTo, NavigationParameters parameters )
        {
            await NavigationAwareHandler( page, parameters, navigatedTo );
            await NavigationAwareHandler( page?.BindingContext, parameters, navigatedTo );
        }

        /// <summary>
        /// Handles the actual NavigationAware calls to the specified object
        /// </summary>
        /// <param name="obj">Potentially NavigationAware object</param>
        /// <param name="parameters">Navigation Parameters to pass the object</param>
        /// <param name="navigatedTo">Is the object being Navigated To or From</param>
        protected async Task NavigationAwareHandler( object obj, NavigationParameters parameters, bool navigatedTo )
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
                        await awareAsyncObj.OnInternalNavigatedToAsync( parameters );
                }
                else
                {
                    awareObj?.OnInternalNavigatedFrom( parameters );
                    if( awareAsyncObj != null )
                        await awareAsyncObj.OnInternalNavigatedFromAsync( parameters );
                }
            }
            catch( Exception e )
            {
                _logger?.Log( e.ToString(), Category.Exception, Priority.None );
            }
        }
    }
}

