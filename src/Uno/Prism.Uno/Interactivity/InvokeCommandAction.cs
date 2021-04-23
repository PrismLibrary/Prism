using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Microsoft.Xaml.Interactivity;

#if HAS_UWP
using Windows.UI.Xaml;
#elif HAS_WINUI
using Microsoft.UI.Xaml;
#endif


namespace Prism.Interactivity
{
    /// <summary>
    /// Trigger action that executes a command when invoked. 
    /// It also maintains the Enabled state of the target control based on the CanExecute method of the command.
    /// </summary>
    public partial class InvokeCommandAction : DependencyObject, IAction
    {
        private ExecutableCommandBehavior _commandBehavior;

        /// <summary>
        /// Dependency property identifying if the associated element should automatically be enabled or disabled based on the result of the Command's CanExecute
        /// </summary>
        public static readonly DependencyProperty AutoEnableProperty =
            DependencyProperty.Register("AutoEnable", typeof(bool), typeof(InvokeCommandAction),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether or not the associated element will automatically be enabled or disabled based on the result of the commands CanExecute
        /// </summary>
        public bool AutoEnable
        {
            get { return (bool)GetValue(AutoEnableProperty); }
            set { SetValue(AutoEnableProperty, value); }
        }

        /// <summary>
        /// Dependency property identifying the command to execute when invoked.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to execute when invoked.
        /// </summary>
        public ICommand Command
        {
            get { return GetValue(CommandProperty) as ICommand; }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Dependency property identifying the command parameter to supply on command execution.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter to supply on command execution.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Dependency property identifying the TriggerParameterPath to be parsed to identify the child property of the trigger parameter to be used as the command parameter.
        /// </summary>
        public static readonly DependencyProperty TriggerParameterPathProperty =
            DependencyProperty.Register("TriggerParameterPath", typeof(string), typeof(InvokeCommandAction),
                new PropertyMetadata(null, (d, e) => { }));

        /// <summary>
        /// Gets or sets the TriggerParameterPath value.
        /// </summary>
        public string TriggerParameterPath
        {
            get { return GetValue(TriggerParameterPathProperty) as string; }
            set { SetValue(TriggerParameterPathProperty, value); }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Microsoft.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <returns>True if the command is successfully executed; else false.</returns>
        public object Execute(object sender, object parameter)
        {
            if (!string.IsNullOrEmpty(TriggerParameterPath))
            {
                //Walk the ParameterPath for nested properties.
                var propertyPathParts = TriggerParameterPath.Split('.');
                object propertyValue = parameter;
                foreach (var propertyPathPart in propertyPathParts)
                {
                    var propInfo = propertyValue.GetType().GetTypeInfo().GetProperty(propertyPathPart);
                    propertyValue = propInfo.GetValue(propertyValue);
                }
                parameter = propertyValue;
            }

            var behavior = GetOrCreateBehavior(sender as UIElement);

            if (behavior != null)
            {
                behavior.ExecuteCommand(parameter);
            }

            return true;
        }

        private ExecutableCommandBehavior GetOrCreateBehavior(UIElement sender)
        {
            // In case this method is called prior to this action being attached, 
            // the CommandBehavior would always keep a null target object (which isn't changeable afterwards).
            // Therefore, in that case the behavior shouldn't be created and this method should return null.
            if (_commandBehavior == null && sender != null)
            {
                _commandBehavior = new ExecutableCommandBehavior(sender);
            }

            _commandBehavior.Command = Command;
            _commandBehavior.CommandParameter = CommandParameter;
            _commandBehavior.AutoEnable = AutoEnable;

            return _commandBehavior;
        }

        /// <summary>
        /// A CommandBehavior that exposes a public ExecuteCommand method. It provides the functionality to invoke commands and update Enabled state of the target control.
        /// It is not possible to make the <see cref="InvokeCommandAction"/> inherit from <see cref="CommandBehaviorBase{T}"/>, since the <see cref="InvokeCommandAction"/>
        /// must already inherit from <see cref="TriggerAction{T}"/>, so we chose to follow the aggregation approach.
        /// </summary>
        private class ExecutableCommandBehavior : CommandBehaviorBase<UIElement>
        {
            /// <summary>
            /// Constructor specifying the target object.
            /// </summary>
            /// <param name="target">The target object the behavior is attached to.</param>
            public ExecutableCommandBehavior(UIElement target)
                : base(target)
            {
            }

            /// <summary>
            /// Executes the command, if it's set.
            /// </summary>
            public new void ExecuteCommand(object parameter)
            {
                base.ExecuteCommand(parameter);
            }
        }
    }
}
