using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Properties;
using System.Threading;
using System.Reflection;
using System.Collections.Specialized;

namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand, IActiveAware
    {
        private bool _isActive;

        private SynchronizationContext _synchronizationContext;

        readonly HashSet<string> _propertiesToObserve = new HashSet<string>();
        private INotifyPropertyChanged _inpc;

        readonly Dictionary<string, CollectionInfo> _collectionsToObserve = new Dictionary<string, CollectionInfo>();

        [CLSCompliant(false)] // Non-private identifier beginning with underscore breaks compliance.
        protected readonly Func<object, Task> _executeMethod;
        [CLSCompliant(false)] // Non-private identifier beginning with underscore breaks compliance.
        protected Func<object, bool> _canExecuteMethod;

        /// <summary>
        /// Creates a new instance of a <see cref="DelegateCommandBase"/>, specifying both the execute action and the can execute function.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{Object,Bool}"/> to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            _executeMethod = (arg) => { executeMethod(arg); return Task.Delay(0); };
            _canExecuteMethod = canExecuteMethod;
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="DelegateCommandBase"/>, specifying both the Execute action as an awaitable Task and the CanExecute function.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Func{Object,Task}"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{Object,Bool}"/> to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public virtual event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> so every 
        /// command invoker can requery <see cref="ICommand.CanExecute"/>.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
                    _synchronizationContext.Post((o) => handler.Invoke(this, EventArgs.Empty), null);
                else
                    handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises <see cref="DelegateCommandBase.CanExecuteChanged"/> so every command invoker
        /// can requery to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="DelegateCommandBase.CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        async void ICommand.Execute(object parameter)
        {
            await Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command with the provided parameter by invoking the <see cref="Action{Object}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter"></param>
		protected virtual async Task Execute(object parameter)
        {
            await _executeMethod(parameter);
        }

        /// <summary>
        /// Determines if the command can execute with the provided parameter by invoking the <see cref="Func{Object,Bool}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter">The parameter to use when determining if this command can execute.</param>
        /// <returns>Returns <see langword="true"/> if the command can execute.  <see langword="False"/> otherwise.</returns>
        protected virtual bool CanExecute(object parameter)
        {
            return _canExecuteMethod(parameter);
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        protected internal void ObservesPropertyInternal<T>(Expression<Func<T>> propertyExpression)
        {
            AddPropertyToObserve(PropertySupport.ExtractPropertyName(propertyExpression));
            HookInpc(propertyExpression.Body as MemberExpression);
        }

        /// <summary>
        /// Observes a collection that implements INotifyCollectionChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on collection changed notifications.
        /// </summary>
        /// <typeparam name="TP">The object type containing the collection property specified in the expression.</typeparam>
        /// <param name="collectionExpression">The collection expression. Example: ObservesProperty(() => CollectionPropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        protected internal void ObservesCollectionInternal<TP>(Expression<Func<TP>> collectionExpression)
        {
            string propertyName = PropertySupport.ExtractPropertyName(collectionExpression);
            MemberExpression memberExpression = collectionExpression.Body as MemberExpression;
            AddPropertyToObserve(propertyName);
            
            HookInpc(memberExpression);
            HookIncc(memberExpression, propertyName);
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute((o) => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        protected internal void ObservesCanExecuteInternal(Expression<Func<object, bool>> canExecuteExpression)
        {
            _canExecuteMethod = canExecuteExpression.Compile();
            AddPropertyToObserve(PropertySupport.ExtractPropertyNameFromLambda(canExecuteExpression));
            HookInpc(canExecuteExpression.Body as MemberExpression);
        }

        protected void HookInpc(MemberExpression expression)
        {
            if (expression == null)
                return;

            if (_inpc == null)
            {
                var constantExpression = expression.Expression as ConstantExpression;
                if (constantExpression != null)
                {
                    _inpc = constantExpression.Value as INotifyPropertyChanged;
                    if (_inpc != null)
                        _inpc.PropertyChanged += Inpc_PropertyChanged;
                }
            }
        }

        protected void HookIncc(MemberExpression expression, string propertyName)
        {
            if (expression == null)
                return;

            if (_inpc != null)
            {
                PropertyInfo property = expression.Member as PropertyInfo;
                if (property != null)
                {
                    var incc = property.GetValue(_inpc) as INotifyCollectionChanged;
                    if (incc != null)
                        incc.CollectionChanged += Incc_CollectionChanged;

                    _collectionsToObserve.Add(propertyName, new CollectionInfo(property, incc));
                }
            }
        }

        protected void AddPropertyToObserve(string property)
        {
            if (_propertiesToObserve.Contains(property))
                throw new ArgumentException(String.Format("{0} is already being observed.", property));

            _propertiesToObserve.Add(property);
        }

        void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the observed collection changes then we have to remove the notify collection 
            // changed listener and start listening on the new collection instead.
            if (_collectionsToObserve.ContainsKey(e.PropertyName))
            {
                CollectionInfo collectionInfo = _collectionsToObserve[e.PropertyName];

                if (collectionInfo.Collection != null)
                    collectionInfo.Collection.CollectionChanged -= Incc_CollectionChanged;

                var incc = collectionInfo.Property.GetValue(_inpc) as INotifyCollectionChanged;
                if (incc != null)
                    incc.CollectionChanged += Incc_CollectionChanged;

                _collectionsToObserve[e.PropertyName] = new CollectionInfo(collectionInfo.Property, incc);
            }


            if (_propertiesToObserve.Contains(e.PropertyName))
                RaiseCanExecuteChanged();
        }

        private void Incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        #region IsActive
        /// <summary>
        /// Gets or sets a value indicating whether the object is active.
        /// </summary>
        /// <value><see langword="true" /> if the object is active; otherwise <see langword="false" />.</value>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnIsActiveChanged();
                }
            }
        }

        /// <summary>
        /// Fired if the <see cref="IsActive"/> property changes.
        /// </summary>
        public virtual event EventHandler IsActiveChanged;

        /// <summary>
        /// This raises the <see cref="DelegateCommandBase.IsActiveChanged"/> event.
        /// </summary>
        protected virtual void OnIsActiveChanged()
        {
            EventHandler isActiveChangedHandler = IsActiveChanged;
            if (isActiveChangedHandler != null)
                isActiveChangedHandler(this, EventArgs.Empty);
        }
        #endregion
    }
}