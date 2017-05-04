using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows.Input;
using Prism.Mvvm;
using System.Threading;

namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand, IActiveAware
    {
        private bool _isActive;

        private SynchronizationContext _synchronizationContext;

        private readonly HashSet<Tuple<object, string>> _propertiesToObserve = new HashSet<Tuple<object, string>>();
        private readonly HashSet<INotifyPropertyChanged> _inpc = new HashSet<INotifyPropertyChanged>();


        /// <summary>
        /// Creates a new instance of a <see cref="DelegateCommandBase"/>, specifying both the execute action and the can execute function.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{Object,Bool}"/> to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase()
        {
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
        /// <remarks>Note that this will trigger the execution of <see cref="DelegateCommandBase.InvokeCanExecute"/> once for each invoker.</remarks>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        protected abstract void Execute(object parameter);

        protected abstract bool CanExecute(object parameter);

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        protected internal void ObservesPropertyInternal<T>(Expression<Func<T>> propertyExpression)
        {
            var constantExpression = (propertyExpression?.Body as MemberExpression)?.Expression as ConstantExpression;
            var objectToObserve = constantExpression?.Value;
            ObservesPropertyInternal(objectToObserve, PropertySupport.ExtractPropertyName(propertyExpression));
        }

        protected internal void ObservesPropertyInternal<T, TProp>(T target, Expression<Func<T, TProp>> propertyExpression)
        {
            ObservesPropertyInternal(target, PropertySupport.ExtractPropertyName(propertyExpression));
        }

        private void ObservesPropertyInternal(object target, string propertyName)
        {
            AddPropertyToObserve(target, propertyName);
            HookInpc(target);
        }

        protected void HookInpc(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var notifyPropertyChangedObject = target as INotifyPropertyChanged;

            if (notifyPropertyChangedObject == null)
                throw new InvalidOperationException(string.Format("{0} object must implement INotifyPropertyChanged.", nameof(target)));

            if (!_inpc.Contains(notifyPropertyChangedObject))
            {
                notifyPropertyChangedObject.PropertyChanged += Inpc_PropertyChanged;
                _inpc.Add(notifyPropertyChangedObject);
            }
        }

        protected void AddPropertyToObserve(object target, string property)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException(nameof(property));

            var propertyToObserve = new Tuple<object, string>(target, property);

            if (_propertiesToObserve.Contains(propertyToObserve))
                throw new ArgumentException(String.Format("{0} of {1} is already being observed.", property, target.GetType().Name));

            _propertiesToObserve.Add(propertyToObserve);
        }

        void Inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_propertiesToObserve.Contains(new Tuple<object, string>(sender, e.PropertyName)) || (string.IsNullOrEmpty(e.PropertyName) && _propertiesToObserve.Count > 0))
            {
                RaiseCanExecuteChanged();
            }
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
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}