using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Properties;

#nullable enable
namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute(T)"/> and <see cref="CanExecute(T)"/>.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    /// <remarks>
    /// The constructor deliberately prevents the use of value types.
    /// Because ICommand takes an object, having a value type for T would cause unexpected behavior when CanExecute(null) is called during XAML initialization for command bindings.
    /// Using default(T) was considered and rejected as a solution because the implementor would not be able to distinguish between a valid and defaulted values.
    /// <para/>
    /// Instead, callers should support a value type by using a nullable value type and checking the HasValue property before using the Value property.
    /// <example>
    ///     <code>
    /// public MyClass()
    /// {
    ///     this.submitCommand = new DelegateCommand&lt;int?&gt;(this.Submit, this.CanSubmit);
    /// }
    /// 
    /// private bool CanSubmit(int? customerId)
    /// {
    ///     return (customerId.HasValue &amp;&amp; customers.Contains(customerId.Value));
    /// }
    ///     </code>
    /// </example>
    /// </remarks>
    public class DelegateCommand<T> : DelegateCommandBase
    {
        readonly Action<T> _executeMethod;
        Func<T, bool> _canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><see cref="CanExecute(T)"/> will always return true.</remarks>
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <exception cref="ArgumentNullException">When both <paramref name="executeMethod"/> and <paramref name="canExecuteMethod"/> are <see langword="null" />.</exception>
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base()
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            TypeInfo genericTypeInfo = typeof(T).GetTypeInfo();

            // DelegateCommand allows object or Nullable<>.  
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericTypeInfo.IsValueType)
            {
                if ((!genericTypeInfo.IsGenericType) || (!typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo())))
                {
                    throw new InvalidCastException(Resources.DelegateCommandInvalidGenericPayloadType);
                }
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        ///<summary>
        ///Executes the command and invokes the <see cref="Action{T}"/> provided during construction.
        ///</summary>
        ///<param name="parameter">Data used by the command.</param>
        public void Execute(T parameter)
        {
            try
            {
                _executeMethod(parameter);
            }
            catch (Exception ex)
            {
                if (!ExceptionHandler.CanHandle(ex))
                    throw;

                ExceptionHandler.Handle(ex, parameter);
            }
        }

        ///<summary>
        ///Determines if the command can execute by invoked the <see cref="Func{T,Bool}"/> provided during construction.
        ///</summary>
        ///<param name="parameter">Data used by the command to determine if it can execute.</param>
        ///<returns>
        ///<see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        ///</returns>
        public bool CanExecute(T parameter)
        {
            try
            {
                return _canExecuteMethod(parameter);
            }
            catch (Exception ex)
            {
                if (!ExceptionHandler.CanHandle(ex))
                    throw;

                ExceptionHandler.Handle(ex, parameter);

                return false;
            }
        }

        /// <summary>
        /// Handle the internal invocation of <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter">Command Parameter</param>
        protected override void Execute(object? parameter)
        {
            try
            {
                // Note: We don't call Execute because we would potentially invoke the Try/Catch twice.
                // It is also needed here incase (T)parameter throws the exception
                _executeMethod((T)parameter!);
            }
            catch (Exception ex)
            {
                if (!ExceptionHandler.CanHandle(ex))
                    throw;

                ExceptionHandler.Handle(ex, parameter);
            }
        }

        /// <summary>
        /// Handle the internal invocation of <see cref="ICommand.CanExecute(object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true"/> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected override bool CanExecute(object? parameter)
        {
            try
            {
                // Note: We don't call Execute because we would potentially invoke the Try/Catch twice.
                // It is also needed here incase (T)parameter throws the exception
                return CanExecute((T)parameter!);
            }
            catch (Exception ex)
            {
                if (!ExceptionHandler.CanHandle(ex))
                    throw;

                ExceptionHandler.Handle(ex, parameter);

                return false;
            }
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="TType">The type of the return value of the method that this delegate encapsulates</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(canExecuteExpression.Body, Expression.Parameter(typeof(T), "o"));
            _canExecuteMethod = expression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch(Action<Exception> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch(Action<Exception, object> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch<TException>(Action<TException> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch<TException>(Action<TException, object> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch(Func<Exception, Task> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch(Func<Exception, object, Task> @catch)
        {
            ExceptionHandler.Register<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch<TException>(Func<TException, Task> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }

        /// <summary>
        /// Registers an async callback if an exception is encountered while executing the <see cref="DelegateCommand"/>
        /// </summary>
        /// <typeparam name="TException">The Exception Type</typeparam>
        /// <param name="catch">The Callback</param>
        /// <returns>The current instance of <see cref="DelegateCommand"/></returns>
        public DelegateCommand<T> Catch<TException>(Func<TException, object, Task> @catch)
            where TException : Exception
        {
            ExceptionHandler.Register<TException>(@catch);
            return this;
        }
    }
}
